using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the Auto generation of Lesson
    /// </summary>
    public interface ILessonGenerationService
    {
        /// <summary>
        /// Auto generates lesson data for a Term into the Lesson table from a LessonTemplate
        /// </summary>
        /// <param name="institutionId">The specific Institution</param>
        /// <param name="dto">The dto containing the term id and a list of template ids</param>
        /// <returns>The number of lessons generated</returns>
        public Task<int> GenerateForTermAsync(Guid institutionId, GenerateLessonDTO dto);

        /// <summary>
        /// Deletes all the generated lessons from one or more template(s)
        /// </summary>
        /// <param name="dto">The template ids dto</param>
        /// <returns>The count of lessons deleted</returns>
        public Task<int> DeleteGeneratedLessons(DeleteLessonDTO dto);
    }
}
