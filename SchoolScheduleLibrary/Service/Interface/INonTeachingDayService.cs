using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for NonTeachingDay
    /// </summary>
    public interface INonTeachingDayService
    {
        /// <summary>
        /// Gets all of the non teaching days
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <returns>A list of non teaching days</returns>
        public Task<List<NonTeachingDayDTO>> GetAllAsync(Guid institutionId);

        /// <summary>
        /// Gets a specific non teaching day from an Id
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The non teaching day's Id</param>
        /// <returns>The specific non teaching day</returns>
        public Task<NonTeachingDayDTO> GetByIdAsync(Guid institutionId, Guid id);

        /// <summary>
        /// Creates a Non teaching day
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The data used to create a non-teaching day</param>
        /// <returns>The created non teaching day</returns>
        public Task<NonTeachingDayDTO> CreateAsync(Guid institutionId, CreateNonTeachingDayDTO dto);

        /// <summary>
        /// Updates an existing non teaching day with new data
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The new values to update the non teaching day with</param>
        /// <returns>The updated non-teaching day</returns>
        public Task<NonTeachingDayDTO> UpdateAsync(Guid institutionId, NonTeachingDayDTO dto);

        /// <summary>
        /// Deletes a non teaching day
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The specific non teaching day's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
