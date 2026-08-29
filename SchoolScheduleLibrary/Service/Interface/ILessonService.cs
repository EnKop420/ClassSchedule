using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the GET business logic for Lesson
    /// </summary>
    public interface ILessonService
    {
        /// <summary>
        /// Gets a list of all the students that has is attending this lesson
        /// </summary>
        /// <param name="institutionId">The institution to look in</param>
        /// <param name="lessonId">The specific lesson</param>
        /// <returns>A list of all the students</returns>
        public Task<List<MinimalUserInformationDTO>> GetStudentsFromSchedule(Guid institutionId, Guid lessonId);
    }
}
