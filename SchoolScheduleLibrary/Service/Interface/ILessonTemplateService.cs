using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface ILessonTemplateService
    {
        public Task<LessonTemplateDTO> CreateAsync(Guid institutionId, CreateLessonTemplateDTO dto);
        public Task<LessonTemplateDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<List<LessonTemplateDTO>> GetAllAsync(Guid institutionId);
        public Task<LessonTemplateDTO> UpdateAsync(Guid institutionId, UpdateLessonTemplateDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
