using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomGenericRepository;

        public RoomService(IGenericRepository<Room> genericRepository)
        {
            _roomGenericRepository = genericRepository;
        }
        public async Task<RoomDTO> CreateAsync(Guid institutionId, CreateRoomDTO dto)
        {
            Room room = new(dto.Name, dto.Capacity, institutionId);
            await _roomGenericRepository.Add(room);
            return new RoomDTO(room.Id, room.Name, room.Capacity);
        }
        
        public async Task<List<RoomDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _roomGenericRepository.GetAll())
                .Where(r => r.InstitutionId == institutionId)
                .Select(r => new RoomDTO(r.Id, r.Name, r.Capacity)).ToList();
        }

        public async Task<RoomDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Room room = await _roomGenericRepository.Get(r => r.Id == id && r.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Room with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new RoomDTO(room.Id, room.Name, room.Capacity);
        }
        public async Task<RoomDTO> UpdateAsync(Guid institutionId, RoomDTO dto)
        {
            Room room = await _roomGenericRepository.Get(r => r.Id == dto.Id && r.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Room with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            room.Name = dto.Name;
            room.Capacity = dto.Capacity;

            Room updatedRoom = await _roomGenericRepository.Update(room);

            return new RoomDTO(updatedRoom.Id, updatedRoom.Name, updatedRoom.Capacity);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _roomGenericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find Room with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _roomGenericRepository.Delete(r => r.Id == id);
        }
    }
}
