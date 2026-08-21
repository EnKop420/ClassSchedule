using SchoolScheduleLibrary.DTO;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IStudentGroupService
    {
        public Task<List<StudentGroupDTO>> GetAllAsync(Guid institutionId);
        public Task<StudentGroupDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<StudentGroupDTO> CreateAsync(Guid institutionId, CreateStudentGroupDTO dto);
        public Task<StudentGroupDTO> UpdateAsync(Guid institutionId, UpdateStudentGroupDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
