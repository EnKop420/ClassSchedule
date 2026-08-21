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
    public class TermService : ITermService
    {
        private readonly IGenericRepository<Term> _termGenericRepository;

        public TermService(IGenericRepository<Term> genericRepository)
        {
            _termGenericRepository = genericRepository;
        }
        public async Task<List<TermDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _termGenericRepository.GetAll())
                .Where(t => t.InstitutionId == institutionId)
                .Select(t => new TermDTO(t.Id, t.Name, t.StartDate, t.EndDate)).ToList();
        }

        public async Task<TermDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Term term = await _termGenericRepository.Get(t => t.Id == id && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new TermDTO(term.Id, term.Name, term.StartDate, term.StartDate);
        }

        public async Task<TermDTO> CreateAsync(Guid institutionId, CreateTermDTO dto)
        {
            Term term = new(dto.Name, dto.StartDate, dto.EndDate, institutionId);

            // Check dates are valid.
            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Start date has to be before End date!");

            bool doesDatesOverlap = await _termGenericRepository.DoesValueExist(t =>
                t.InstitutionId == institutionId
                && t.StartDate <= dto.EndDate
                && t.EndDate >= dto.StartDate);

            if (doesDatesOverlap) throw new BadRequestException("Dates overlap with an existing Term");

            await _termGenericRepository.Add(term);
            return new TermDTO(term.Id, term.Name, term.StartDate, term.EndDate);
        }
        public async Task<TermDTO> UpdateAsync(Guid institutionId, TermDTO dto)
        {
            Term term = await _termGenericRepository.Get(t => t.Id == dto.Id && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            // Check dates are valid.
            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Start date has to be before End date!");
            bool doesDatesOverlap = await _termGenericRepository.DoesValueExist(t =>
                t.InstitutionId == institutionId
                && t.StartDate <= dto.EndDate
                && t.EndDate >= dto.StartDate);

            if (doesDatesOverlap) throw new BadRequestException("Dates overlap with an existing Term");

            term.Name = dto.Name;
            term.StartDate = dto.StartDate;
            term.EndDate = dto.EndDate;

            Term updatedTerm = await _termGenericRepository.Update(term);

            return new TermDTO(updatedTerm.Id, updatedTerm.Name, updatedTerm.StartDate, updatedTerm.EndDate);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _termGenericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find Term with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _termGenericRepository.Delete(t => t.Id == id);
        }
    }
}
