using SchoolScheduleLibrary.DTO;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IStudentGroupMemberService
    {
        public Task<bool> AddStudentListAsync(Guid institutionId, Guid StudentGroupId, List<Guid> studentIds);
        public Task<bool> RemoveStudentListAsync(Guid institutionId, Guid studentGroupId, List<Guid> studentIds);
        public Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid StudentGroupId);
    }
}
