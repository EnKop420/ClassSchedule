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
    public class NonTeachingDayService : INonTeachingDayService
    {
        private readonly IGenericRepository<NonTeachingDay> _nonTeachingDayGenericRepository;
        public NonTeachingDayService(IGenericRepository<NonTeachingDay> nonTeachingDayGenericRepository)
        {
            _nonTeachingDayGenericRepository = nonTeachingDayGenericRepository;
        }

        public async Task<List<NonTeachingDayDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _nonTeachingDayGenericRepository.GetAll())
                .Where(p => p.InstitutionId == institutionId)
                .Select(ntd => new NonTeachingDayDTO(ntd.Id, ntd.StartDate, ntd.EndDate, ntd.Reason)).ToList();
        }

        public async Task<NonTeachingDayDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            NonTeachingDay nonTeachingDay = await _nonTeachingDayGenericRepository.Get(ntd => ntd.Id == id && ntd.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get NonTeachingDay with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new NonTeachingDayDTO(nonTeachingDay.Id, nonTeachingDay.StartDate, nonTeachingDay.EndDate, nonTeachingDay.Reason);
        }

        public async Task<NonTeachingDayDTO> CreateAsync(Guid institutionId, CreateNonTeachingDayDTO dto)
        {
            NonTeachingDay nonTeachingDay = new(dto.StartDate, dto.EndDate, dto.Reason, institutionId);

            // Check dates are valid.
            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Start time has to be before End time!");
            bool doesDatesOverlap = await _nonTeachingDayGenericRepository.DoesValueExist(ntd => 
                ntd.InstitutionId == institutionId
                && ntd.StartDate <= dto.EndDate
                && ntd.EndDate >= dto.StartDate);

            if (doesDatesOverlap) throw new BadRequestException("Dates overlap with existing Non Teaching Day(s)");

            await _nonTeachingDayGenericRepository.Add(nonTeachingDay);
            return new NonTeachingDayDTO(nonTeachingDay.Id, nonTeachingDay.StartDate, nonTeachingDay.EndDate, nonTeachingDay.Reason);
        }

        public async Task<NonTeachingDayDTO> UpdateAsync(Guid institutionId, NonTeachingDayDTO dto)
        {
            NonTeachingDay nonTeachingDay = await _nonTeachingDayGenericRepository.Get(ntd => ntd.Id == dto.Id && ntd.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get NonTeachingDay with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            // Check dates are valid.
            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Start time has to be before End time!");
            bool doesDatesOverlap = await _nonTeachingDayGenericRepository.DoesValueExist(ntd =>
                ntd.InstitutionId == institutionId
                && ntd.StartDate <= dto.EndDate
                && ntd.EndDate >= dto.StartDate);

            if (doesDatesOverlap) throw new BadRequestException("Dates overlap with existing Non Teaching Day(s)");

            nonTeachingDay.StartDate = dto.StartDate;
            nonTeachingDay.EndDate = dto.EndDate;
            nonTeachingDay.Reason = dto.Reason;

            NonTeachingDay updatedNonTeachingDay = await _nonTeachingDayGenericRepository.Update(nonTeachingDay);

            return new NonTeachingDayDTO(updatedNonTeachingDay.Id, updatedNonTeachingDay.StartDate, updatedNonTeachingDay.EndDate, updatedNonTeachingDay.Reason);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _nonTeachingDayGenericRepository.DoesValueExist(ntd => ntd.Id == id && ntd.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find NonTeachingDay with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _nonTeachingDayGenericRepository.Delete(p => p.Id == id);
        }
    }
}
