using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface ISubjectService
    {
        public Task<List<SubjectDTO>> GetAllAsync(Guid institutionId);
        public Task<SubjectDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<SubjectDTO> CreateAsync(Guid institutionId, CreateSubjectDTO dto);
        public Task<SubjectDTO> UpdateAsync(Guid institutionId, SubjectDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
