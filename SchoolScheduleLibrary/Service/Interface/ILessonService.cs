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
        /// <param name="lessonId">The specific lesson</param>
        /// <returns>A list of all the students</returns>
        public Task<List<MinimalUserInformationDTO>> GetStudentsFromSchedule(Guid lessonId);

        /// <summary>
        /// Changes the status on a lesson
        /// </summary>
        /// <param name="lessonId">The specific lesson</param>
        /// <param name="status">The status</param>
        /// <returns>A True or False statement if the statement </returns>
        public Task<bool> ChangeLessonStatus(Guid lessonId, LessonStatus status);
    }
}
