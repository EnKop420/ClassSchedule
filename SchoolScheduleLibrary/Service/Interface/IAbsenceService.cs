using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IAbsenceService
    {
        public Task<bool> SetAbsence(Guid lessonId, List<SetAbsenceDTO> dtos, Guid teacherId);
        public Task<List<AbsenceDTO>> GetAllAbsencesFromLesson(Guid lessonId);
        //public Task<List<AbsenceDTO>> GetAbsencesFromStudent(Guid studentId);
    }
}
