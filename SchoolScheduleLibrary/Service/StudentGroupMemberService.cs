using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
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

        public async Task<bool> AddStudentListAsync(Guid institutionId, Guid studentGroupId, List<Guid> studentIds)
        {
            bool doesStudentGroupExist = await _studentGroupGenericRepository.DoesValueExist(sg => sg.Id == studentGroupId && sg.InstitutionId == institutionId);
            if (doesStudentGroupExist == false) throw new NotFoundException($"Could not get Student Group with Id \"{studentGroupId}\" in the Institution with Id \"{institutionId}\"");

            List<StudentGroupMember> studentGroupMembers = [];

            foreach (Guid student in studentIds.Distinct())
            {
                User user = await _userGenericRepository.Get(u => u.Id == student && u.InstitutionId == institutionId)
                    ?? throw new NotFoundException($"Could not get Student with Id \"{student}\" in the Institution with Id \"{institutionId}\"");

                if (user.Role != UserRoles.Student) throw new BadRequestException("User is not a student!");

                bool isStudentInStudentGroup = await _studentGroupMemberGenericRepository.DoesValueExist(sgm =>
                    sgm.StudentGroupId == studentGroupId && sgm.StudentId == student);

                if (isStudentInStudentGroup)
                {
                    throw new ConflictException($"{user.FirstName} {user.LastName} {student} Student is already in this student group");
                }

                studentGroupMembers.Add(new(studentGroupId, student));
            }

            if (studentGroupMembers.Count == 0)
            {
                return true;
            }

            return await _studentGroupMemberGenericRepository.AddRange(studentGroupMembers);
        }

        public async Task<List<StudentGroupMemberDTO>> GetStudentsAsync(Guid studentGroupId)
        {
            return (await _studentGroupMemberGenericRepository.GetAll(sgm => sgm.StudentGroupId == studentGroupId,sgm => sgm.Student))
                .Select(sgm => new StudentGroupMemberDTO(
                    $"{sgm.Student.FirstName} {sgm.Student.LastName}",
                    sgm.StudentId
                )).ToList();
        }

        public async Task<bool> RemoveStudentAsync(Guid institutionId, Guid studentGroupId, Guid studentId)
        {
            bool doesStudentGroupExist = await _studentGroupGenericRepository.DoesValueExist(sg => 
                sg.Id == studentGroupId && sg.InstitutionId == institutionId);

            if (!doesStudentGroupExist) throw new NotFoundException($"Could not get Student Group with Id \"{studentGroupId}\" " +
                    $"in the Institution with Id \"{institutionId}\"");

            bool isStudentInStudentGroup = await _studentGroupMemberGenericRepository.DoesValueExist(sgm =>
                sgm.StudentGroupId == studentGroupId && sgm.StudentId == studentId);

            if (isStudentInStudentGroup == false) throw new BadRequestException("Student is not a member of this student group");

            return await _studentGroupMemberGenericRepository.Delete(sgm => sgm.StudentGroupId == studentGroupId && sgm.StudentId == studentId);
        }
    }
}