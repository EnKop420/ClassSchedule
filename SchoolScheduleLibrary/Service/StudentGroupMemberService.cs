using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Service.Interface;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class StudentGroupMemberService : IStudentGroupMemberService
    {
        private readonly IGenericRepository<StudentGroup> _studentGroupGenericRepository;
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IGenericRepository<StudentGroupMember> _studentGroupMemberGenericRepository;

        public StudentGroupMemberService(
            IGenericRepository<StudentGroup> studentGroupGenericRepository,
            IGenericRepository<User> userGenericRepository,
            IGenericRepository<StudentGroupMember> studentGroupMemberGenericRepository)
        {
            _studentGroupGenericRepository = studentGroupGenericRepository;
            _userGenericRepository = userGenericRepository;
            _studentGroupMemberGenericRepository = studentGroupMemberGenericRepository;
        }

        public async Task<bool> AddStudentListAsync(Guid institutionId, Guid StudentGroupId, List<Guid> studentIds)
        {
            bool doesStudentGroupExist = await _studentGroupGenericRepository.DoesValueExist(sg => sg.Id == StudentGroupId && sg.InstitutionId == institutionId);
            if (doesStudentGroupExist == false) throw new NotFoundException($"Could not get Student Group with Id \"{StudentGroupId}\" in the Institution with Id \"{institutionId}\"");

            List<StudentGroupMember> studentGroupMembers = [];


            foreach (Guid student in studentIds)
            {
                User user = await _userGenericRepository.Get(u => u.Id == student && u.InstitutionId == institutionId)
                    ?? throw new NotFoundException($"Could not get Student with Id \"{student}\" in the Institution with Id \"{institutionId}\"");
                if (user.Role != UserRoles.Student) throw new BadRequestException("User is not a student!");

                bool isStudentEnrolled = await _studentGroupMemberGenericRepository.DoesValueExist(sgm =>
                    sgm.StudentGroupId == StudentGroupId && sgm.StudentId == student);

                if (isStudentEnrolled)
                {
                    throw new ConflictException($"{user.FirstName} {user.LastName} {student} Student is already enrolled into this student group");
                }
                else
                {
                    studentGroupMembers.Add(new(StudentGroupId, student));
                }
            }

            return await _studentGroupMemberGenericRepository.AddRange(studentGroupMembers);
        }

        public async Task<List<StudentGroupMemberDTO>> GetStudentsAsync(Guid studentGroupId)
        {
            List<StudentGroupMemberDTO> students = (await _studentGroupMemberGenericRepository.GetAll(sgm => sgm.StudentGroupId == studentGroupId, sgm => sgm.Student))
                .Select(t => new StudentGroupMemberDTO($"{t.Student.FirstName} {t.Student.LastName}", studentGroupId, t.StudentId)).ToList();

            return students;
        }

        public async Task<bool> RemoveStudentAsync(Guid institutionId, Guid StudentGroupId, Guid studentId)
        {
            bool isStudentInStudentGroup = await _studentGroupMemberGenericRepository.DoesValueExist(sgm =>
                sgm.StudentGroupId == StudentGroupId && sgm.StudentId == studentId);

            if (isStudentInStudentGroup == false) throw new BadRequestException("Student is not a member of this student group");

            return await _studentGroupMemberGenericRepository.Delete(sgm => sgm.StudentGroupId == StudentGroupId && sgm.StudentId == studentId);
        }
    }
}