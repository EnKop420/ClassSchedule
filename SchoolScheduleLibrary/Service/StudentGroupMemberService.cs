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
                User user = await _userGenericRepository.Get(u => u.Id == student && u.InstitutionId == institutionId && u.Role == UserRoles.Student)
                    ?? throw new NotFoundException($"Could not get Student with Id \"{student}\" in the Institution with Id \"{institutionId}\"");

                bool isStudentInStudentGroup = await _studentGroupMemberGenericRepository.DoesValueExist(sgm =>
                    sgm.StudentGroupId == studentGroupId && sgm.StudentId == student);

                if (isStudentInStudentGroup)
                {
                    throw new ConflictException($"Student with id {student} is already in this student group");
                }

                studentGroupMembers.Add(new(studentGroupId, student));
            }

            return await _studentGroupMemberGenericRepository.AddRange(studentGroupMembers);
        }

        public async Task<bool> RemoveStudentListAsync(Guid institutionId, Guid studentGroupId, List<Guid> studentIds)
        {
            bool doesStudentGroupExist = await _studentGroupGenericRepository.DoesValueExist(sg => 
                sg.Id == studentGroupId && sg.InstitutionId == institutionId);

            if (!doesStudentGroupExist) throw new NotFoundException($"Could not get Student Group with Id \"{studentGroupId}\" " +
                    $"in the Institution with Id \"{institutionId}\"");

            List<StudentGroupMember> studentGroupMembers = await _studentGroupMemberGenericRepository.GetAll(sgm => 
                sgm.StudentGroupId == studentGroupId && studentIds.Contains(sgm.StudentId));

            return await _studentGroupMemberGenericRepository.RemoveRange(studentGroupMembers);
        }

        public async Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid studentGroupId)
        {
            return (await _studentGroupMemberGenericRepository.GetAll(sgm => sgm.StudentGroupId == studentGroupId, sgm => sgm.Student))
                .Select(sgm => new MinimalUserInformationDTO(
                    $"{sgm.Student.FirstName} {sgm.Student.LastName}",
                    sgm.StudentId
                )).ToList();
        }
    }
}