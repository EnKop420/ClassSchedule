using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for Hold.
    /// </summary>
    public interface IHoldService
    {
        /// <summary>
        /// Creates the Hold while also adding the students and teachers
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">Contains the Name and list of students and teachers</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> CreateAsync(Guid institutionId, CreateHoldDTO dto);

        /// <summary>
        /// Updates the Hold while also updating the students and teachers
        /// </summary>
        /// <param name="dto">Contains the Id, Name and list of students and teachers</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> UpdateAsync(HoldDTO dto);

        /// <summary>
        /// Deletes the Hold
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The Hold to delete's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Get's the specific Hold from the Id
        /// </summary>
        /// <param name="id">The specific Id of the hold</param>
        /// <returns>Return a HoldDTO</returns>
        public Task<HoldDTO> GetByIdAsync(Guid id);

        /// <summary>
        /// Get's a list of all the Hold's in the institution
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <returns>A list of HoldDTO</returns>
        public Task<List<HoldDTO>> GetAllAsync(Guid institutionId);
    }
}
