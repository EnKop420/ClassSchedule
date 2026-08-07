using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IRoomService
    {
        public Task<List<RoomDTO>> GetAllAsync(Guid institutionId);
        public Task<RoomDTO> GetByIdAsync(Guid institutionId, Guid id);
        public Task<RoomDTO> CreateAsync(Guid institutionId, CreateRoomDTO dto);
        public Task<RoomDTO> UpdateAsync(Guid institutionId, RoomDTO dto);
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);
    }
}
