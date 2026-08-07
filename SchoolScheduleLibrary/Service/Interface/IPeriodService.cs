using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IPeriodService
    {
        public Task<List<PeriodDTO>> GetAllAsync(Guid institutionId);
        public Task<PeriodDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<PeriodDTO> CreateAsync(Guid institutionId, CreatePeriodDTO dto);
        public Task<PeriodDTO> UpdateAsync(Guid institutionId, PeriodDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
