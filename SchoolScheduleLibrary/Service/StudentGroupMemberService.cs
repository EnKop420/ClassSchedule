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
        private readonly IGenericRepository<StudentGroupMember> _studentGroupMemberGenericRepository;

        public StudentGroupMemberService(IGenericRepository<StudentGroupMember> studentGroupMemberGenericRepository)
        {
            _studentGroupMemberGenericRepository = studentGroupMemberGenericRepository;
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