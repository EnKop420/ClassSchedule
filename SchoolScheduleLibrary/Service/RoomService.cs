using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
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
        public async Task<bool> CreateAsync(Guid institutionId, CreateRoomDTO dto)
        {
            Room room = new(dto.Name, dto.Capacity, institutionId);
            return await _roomGenericRepository.Add(room);
        }
        
        public async Task<List<RoomDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _roomGenericRepository.GetAll())
                .Where(r => r.InstitutionId == institutionId)
                .Select(r => new RoomDTO(r.Id, r.Name, r.Capacity)).ToList();
        }

        public async Task<RoomDTO> GetByIdAsync(Guid id)
        {
            Room room = await _roomGenericRepository.Get(r => r.Id == id)
                ?? throw new NotFoundException($"Could not get Room with Id \"{id}\"");

            return new RoomDTO(room.Id, room.Name, room.Capacity);
        }
        public async Task<bool> UpdateAsync(RoomDTO dto)
        {
            Room room = await _roomGenericRepository.Get(r => r.Id == dto.Id)
                ?? throw new NotFoundException($"Could not get Room with Id \"{dto.Id}\"");

            room.Name = dto.Name;
            room.Capacity = dto.Capacity;

            return await _roomGenericRepository.Update(room);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _roomGenericRepository.DoesValueExist(t => t.Id == id))
            {
                throw new NotFoundException($"Could not find Room with Id \"{id}\"");
            }

            return await _roomGenericRepository.Delete(r => r.Id == id);
        }
    }
}
