using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for LessonTemplate
    /// </summary>
    public interface ILessonTemplateService
    {
        /// <summary>
        /// Creates a LessonTemplate
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The data used to create a lesson template</param>
        /// <returns>The created lesson template</returns>
        public Task<LessonTemplateDTO> CreateAsync(Guid institutionId, CreateLessonTemplateDTO dto);

        /// <summary>
        /// Gets a specific lesson template from an Id
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The lesson template's Id</param>
        /// <returns>The specific LessonTemplate</returns>
        public Task<LessonTemplateDTO> GetByIdAsync(Guid institutionId, Guid id);

        /// <summary>
        /// Gets all of the lesson templates
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <returns>A list of lesson templates</returns>
        public Task<List<LessonTemplateDTO>> GetAllAsync(Guid institutionId);

        /// <summary>
        /// Updates an existing LessonTemplate with new data
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The new values to update the lesson template with</param>
        /// <returns>The updated lesson template</returns>
        public Task<LessonTemplateDTO> UpdateAsync(Guid institutionId, UpdateLessonTemplateDTO dto);

        /// <summary>
        /// Deletes a lesson template
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The specific lesson template's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
