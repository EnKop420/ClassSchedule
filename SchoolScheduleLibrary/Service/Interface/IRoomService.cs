using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for Room
    /// </summary>
    public interface IRoomService
    {
        /// <summary>
        /// Gets all of the rooms
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <returns>A list of rooms</returns>
        public Task<List<RoomDTO>> GetAllAsync(Guid institutionId);

        /// <summary>
        /// Gets a specific room from an Id
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The room's Id</param>
        /// <returns>The specific Room</returns>
        public Task<RoomDTO> GetByIdAsync(Guid institutionId, Guid id);

        /// <summary>
        /// Creates a Room
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The data used to create a room</param>
        /// <returns>The created room</returns>
        public Task<RoomDTO> CreateAsync(Guid institutionId, CreateRoomDTO dto);

        /// <summary>
        /// Updates an existing Room with new data
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">The new values to update the room with</param>
        /// <returns>The updated room</returns>
        public Task<RoomDTO> UpdateAsync(Guid institutionId, RoomDTO dto);

        /// <summary>
        /// Deletes a room
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The specific room's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
