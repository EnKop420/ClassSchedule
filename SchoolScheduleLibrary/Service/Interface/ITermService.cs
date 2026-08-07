using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface ITermService
    {
        public Task<List<TermDTO>> GetAllAsync(Guid institutionId);
        public Task<TermDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<TermDTO> CreateAsync(Guid institutionId, CreateTermDTO dto);
        public Task<TermDTO> UpdateAsync(Guid institutionId, TermDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
