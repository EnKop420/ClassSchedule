using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface ISubjectService
    {
        public Task<List<SubjectDTO>> GetAllAsync(Guid institutionId);
        public Task<SubjectDTO> CreateAsync(Guid institutionId, CreateSubjectDTO dto);
    }
}
