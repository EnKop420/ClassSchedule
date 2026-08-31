using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    /// <summary>
    /// Used for specific use cases that could not be done in the Generic Repository.
    /// </summary>
    public interface ILessonRepository
    {
        /// <summary>
        /// Gets all the students for a lesson
        /// </summary>
        /// <param name="lessonId">The specific lesson</param>
        /// <returns>A list of users</returns>
        public Task<List<User>> GetStudentsFromLessonAsync(Guid lessonId);

        /// <summary>
        /// Gets all the lessons for a Student
        /// </summary>
        /// <param name="institutionId">The Institution it should look in.</param>
        /// <param name="dto">The DTO containing all the values needed to get the data.</param>
        /// <returns>Returns a list of Lesson's</returns>
        public Task<List<LessonDTO>> GetStudentLessonsAsync(Guid institutionId, GetLessonDTO dto);

        /// <summary>
        /// Gets all the lessons for a Teacher
        /// </summary>
        /// <param name="institutionId">The Institution it should look in.</param>
        /// <param name="dto">The DTO containing all the values needed to get the data.</param>
        /// <returns>Returns a list of Lesson's</returns>
        public Task<List<LessonDTO>> GetTeacherLessonsAsync(Guid institutionId, GetLessonDTO dto);

        /// <summary>
        /// Gets a specific lesson using the Id
        /// </summary>
        /// <param name="lessonId">The lesson's Id</param>
        /// <returns>A nullable LessonDTO.</returns>
        public Task<LessonDTO?> GetLesson(Guid lessonId);
    }
}
