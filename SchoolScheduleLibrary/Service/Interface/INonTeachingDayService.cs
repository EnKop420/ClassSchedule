using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface INonTeachingDayService
    {
        public Task<List<NonTeachingDayDTO>> GetAllAsync(Guid institutionId);
        public Task<NonTeachingDayDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<NonTeachingDayDTO> CreateAsync(Guid institutionId, CreateNonTeachingDayDTO dto);
        public Task<NonTeachingDayDTO> UpdateAsync(Guid institutionId, NonTeachingDayDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
