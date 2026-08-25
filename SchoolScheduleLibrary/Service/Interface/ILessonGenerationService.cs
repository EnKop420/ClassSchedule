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
        /// <param name="termId">The specific Term</param>
        /// <returns>The number of lessons generated</returns>
        public Task<int> GenerateForTermAsync(Guid institutionId, Guid termId);
    }
}
