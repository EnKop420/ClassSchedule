using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for Term
    /// </summary>
    public interface ITermService
    {
        public Task<List<TermDTO>> GetAllAsync(Guid institutionId);
        public Task<TermDTO> GetByIdAsync(Guid id);
        public Task<bool> CreateAsync(Guid institutionId, CreateTermDTO dto);
        public Task<bool> UpdateAsync(Guid institutionId, TermDTO dto);
        public Task<bool> DeleteAsync(Guid id);
    }
}
