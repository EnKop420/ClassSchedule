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
        private readonly IGenericRepository<Room> _genericRepository;
        public RoomService(IGenericRepository<Room> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<RoomDTO> CreateAsync(Guid institutionId, CreateRoomDTO dto)
        {
            Room room = new Room { Name = dto.Name, Capacity = dto.Capacity, InstitutionId = institutionId };
            await _genericRepository.Create(room);
            return new RoomDTO(room.Id, room.Name, room.Capacity);
        }
        
        public async Task<List<RoomDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _genericRepository.GetAll())
                .Where(r => r.InstitutionId == institutionId)
                .Select(r => new RoomDTO(r.Id, r.Name, r.Capacity)).ToList();
        }

        public async Task<RoomDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Room room = await _genericRepository.GetById(id) ?? throw new NotFoundException($"Room with ID {id} does not exist.");
            if (room.InstitutionId != institutionId) throw new BadRequestException("Room is not apart of the Institution!");

            return new RoomDTO(room.Id, room.Name, room.Capacity);
        }
        public async Task<RoomDTO> UpdateAsync(Guid institutionId, RoomDTO dto)
        {
            Room room = await _genericRepository.GetById(dto.Id) ?? throw new NotFoundException($"Room with ID {dto.Id} does not exist.");
            if (room.InstitutionId != institutionId) throw new BadRequestException("Room is not apart of the Institution!");

            room.Name = dto.Name;
            room.Capacity = dto.Capacity;

            Room updatedRoom = await _genericRepository.Update(room);

            return new RoomDTO(updatedRoom.Id, updatedRoom.Name, updatedRoom.Capacity);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            Room room = await _genericRepository.GetById(id) ?? throw new NotFoundException($"Room with ID {id} does not exist.");
            if (room.InstitutionId != institutionId) throw new BadRequestException("Room is not apart of the Institution!");

            return await _genericRepository.Delete(room);
        }
    }
}
