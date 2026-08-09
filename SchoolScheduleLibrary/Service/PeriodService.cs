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
    public class PeriodService : IPeriodService
    {
        private readonly IGenericRepository<Period> _genericRepository;
        public PeriodService(IGenericRepository<Period> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<List<PeriodDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _genericRepository.GetAll())
                .Where(p => p.InstitutionId == institutionId)
                .Select(p => new PeriodDTO(p.Id, p.Name, p.StartTime, p.EndTime, p.SortOrder)).ToList();
        }

        public async Task<PeriodDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Period period = await _genericRepository.GetById(p => p.Id == id && p.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Period with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime, period.SortOrder);
        }

        public async Task<PeriodDTO> CreateAsync(Guid institutionId, CreatePeriodDTO dto)
        {
            Period period = new(dto.Name, dto.StartTime, dto.EndTime, dto.SortOrder, institutionId);

            // Check dates are valid.
            if (dto.StartTime > dto.EndTime) throw new BadRequestException("Start time has to be before End time!");

            await _genericRepository.Create(period);
            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime, period.SortOrder);
        }
        public async Task<PeriodDTO> UpdateAsync(Guid institutionId, PeriodDTO dto)
        {
            Period period = await _genericRepository.GetById(p => p.Id == dto.Id && p.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Period with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            // Check dates are valid.
            if (dto.StartTime > dto.EndTime) throw new BadRequestException("Start time has to be before End time!");

            period.Name = dto.Name;
            period.StartTime = dto.StartTime;
            period.EndTime = dto.EndTime;

            Period updatedPeriod = await _genericRepository.Update(period);

            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime, period.SortOrder);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _genericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find Period with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _genericRepository.DeleteById(id);
        }
    }
}
