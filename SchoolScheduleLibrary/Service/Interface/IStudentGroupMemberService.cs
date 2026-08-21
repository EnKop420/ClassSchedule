using SchoolScheduleLibrary.DTO;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IStudentGroupMemberService
    {
        public Task<bool> AddStudentListAsync(Guid institutionId, Guid StudentGroupId, List<Guid> studentIds);
        public Task<bool> RemoveStudentAsync(Guid institutionId, Guid StudentGroupId, Guid studentId);
        public Task<List<StudentGroupMemberDTO>> GetStudentsAsync(Guid StudentGroupId);
    }
}
