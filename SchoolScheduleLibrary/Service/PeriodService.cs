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
    public class PeriodService : IPeriodService
    {
        private readonly IGenericRepository<Period> _periodGenericRepository;
        public PeriodService(IGenericRepository<Period> genericRepository)
        {
            _periodGenericRepository = genericRepository;
        }
        public async Task<List<PeriodDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _periodGenericRepository.GetAll())
                .Where(p => p.InstitutionId == institutionId)
                .Select(p => new PeriodDTO(p.Id, p.Name, p.StartTime, p.EndTime)).ToList();
        }

        public async Task<PeriodDTO> GetByIdAsync(Guid id)
        {
            Period period = await _periodGenericRepository.Get(p => p.Id == id)
                ?? throw new NotFoundException($"Could not get Period with Id \"{id}\"");

            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime);
        }

        public async Task<bool> CreateAsync(Guid institutionId, CreatePeriodDTO dto)
        {
            Period period = new(dto.Name, dto.StartTime, dto.EndTime, institutionId);

            // Check dates are valid.
            if (dto.StartTime > dto.EndTime) throw new BadRequestException("Start time has to be before End time!");

            return await _periodGenericRepository.Add(period);
        }

        public async Task<bool> UpdateAsync(PeriodDTO dto)
        {
            Period period = await _periodGenericRepository.Get(p => p.Id == dto.Id)
                ?? throw new NotFoundException($"Could not get Period with Id \"{dto.Id}\"");

            // Check dates are valid.
            if (dto.StartTime > dto.EndTime) throw new BadRequestException("Start time has to be before End time!");

            period.Name = dto.Name;
            period.StartTime = dto.StartTime;
            period.EndTime = dto.EndTime;

            return await _periodGenericRepository.Update(period);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _periodGenericRepository.DoesValueExist(t => t.Id == id))
            {
                throw new NotFoundException($"Could not find Period with Id \"{id}\"");
            }

            return await _periodGenericRepository.Delete(p => p.Id == id);
        }
    }
}
