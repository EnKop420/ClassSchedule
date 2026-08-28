using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for Period
    /// </summary>
    public interface IPeriodService
    {
        /// <summary>
        /// Gets all of the periods
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <returns>A list of periods</returns>
        public Task<List<PeriodDTO>> GetAllAsync(Guid institutionId);

        /// <summary>
        /// Gets a specific period from an Id
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The period's Id</param>
        /// <returns>The specific Period</returns>
        public Task<PeriodDTO> GetByIdAsync(Guid institutionId, Guid id);

        /// <summary>
        /// Creates a Period
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The data used to create a period</param>
        /// <returns>The created period</returns>
        public Task<PeriodDTO> CreateAsync(Guid institutionId, CreatePeriodDTO dto);

        /// <summary>
        /// Updates an existing Period with new data
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The new values to update the period with</param>
        /// <returns>The updated period</returns>
        public Task<PeriodDTO> UpdateAsync(Guid institutionId, PeriodDTO dto);

        /// <summary>
        /// Deletes a period
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The specific period's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
