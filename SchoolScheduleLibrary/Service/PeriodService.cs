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
            Period period = await _genericRepository.GetById(id) ?? throw new NotFoundException($"Period with ID {id} does not exist.");
            if (period.InstitutionId != institutionId) throw new BadRequestException("Period is not apart of the Institution!");

            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime, period.SortOrder);
        }

        public async Task<PeriodDTO> CreateAsync(Guid institutionId, CreatePeriodDTO dto)
        {
            Period period = new Period { Name = dto.Name, StartTime = dto.StartTime, EndTime = dto.EndTime, SortOrder = dto.SortOrder, InstitutionId = institutionId };
            await _genericRepository.Create(period);
            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime, period.SortOrder);
        }
        public async Task<PeriodDTO> UpdateAsync(Guid institutionId, PeriodDTO dto)
        {
            Period period = await _genericRepository.GetById(dto.Id) ?? throw new NotFoundException($"Period with ID {dto.Id} does not exist.");
            if (period.InstitutionId != institutionId) throw new BadRequestException("Period is not apart of the Institution!");

            period.Name = dto.Name;
            period.StartTime = dto.StartTime;
            period.EndTime = dto.EndTime;

            Period updatedPeriod = await _genericRepository.Update(period);

            return new PeriodDTO(period.Id, period.Name, period.StartTime, period.EndTime, period.SortOrder);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            Period period = await _genericRepository.GetById(id) ?? throw new NotFoundException($"Period with ID {id} does not exist.");
            if (period.InstitutionId != institutionId) throw new BadRequestException("Period is not apart of the Institution!");

            return await _genericRepository.Delete(period);
        }
    }
}
