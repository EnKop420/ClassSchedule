using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IHoldService
    {
        public Task<HoldDTO> CreateAsync(Guid institutionId, CreateHoldDTO dto);
        public Task<HoldDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<List<HoldDTO>> GetAllAsync(Guid institutionId);
        public Task<HoldDTO> UpdateAsync(Guid institutionId, HoldDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
