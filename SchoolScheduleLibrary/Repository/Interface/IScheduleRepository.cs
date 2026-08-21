using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScheduleRepository
    {
        public Task<List<Lesson>> GetStudentLessonsAsync(Guid institutionId, GetScheduleLessonDTO dto);
        public Task<List<Lesson>> GetTeacherLessonsAsync(Guid institutionId, GetScheduleLessonDTO dto);
    }
}
