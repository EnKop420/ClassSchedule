using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for Subject 
    /// </summary>
    public interface ISubjectService
    {
        public Task<List<SubjectDTO>> GetAllAsync(Guid institutionId);
        public Task<SubjectDTO> GetByIdAsync(Guid id);
        public Task<bool> CreateAsync(Guid institutionId, CreateSubjectDTO dto);
        public Task<bool> UpdateAsync(SubjectDTO dto);
        public Task<bool> DeleteAsync(Guid id);
    }
}
