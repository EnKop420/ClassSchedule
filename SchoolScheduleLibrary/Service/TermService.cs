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
    public class TermService : ITermService
    {
        private readonly IGenericRepository<Term> _genericRepository;

        public TermService(IGenericRepository<Term> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<List<TermDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _genericRepository.GetAll())
                .Where(t => t.InstitutionId == institutionId)
                .Select(t => new TermDTO(t.Id, t.Name, t.StartDate, t.EndDate)).ToList();
        }

        public async Task<TermDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Term term = await _genericRepository.GetById(t => t.Id == id && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new TermDTO(term.Id, term.Name, term.StartDate, term.StartDate);
        }

        public async Task<TermDTO> CreateAsync(Guid institutionId, CreateTermDTO dto)
        {
            Term term = new(dto.Name, dto.StartDate, dto.EndDate, institutionId);

            // Check dates are valid.
            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Start date has to be before End date!");

            await _genericRepository.Create(term);
            return new TermDTO(term.Id, term.Name, term.StartDate, term.EndDate);
        }
        public async Task<TermDTO> UpdateAsync(Guid institutionId, TermDTO dto)
        {
            Term term = await _genericRepository.GetById(t => t.Id == dto.Id && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            // Check dates are valid.
            if (dto.StartDate > dto.EndDate) throw new BadRequestException("Start date has to be before End date!");

            term.Name = dto.Name;
            term.StartDate = dto.StartDate;
            term.EndDate = dto.EndDate;

            Term updatedTerm = await _genericRepository.Update(term);

            return new TermDTO(updatedTerm.Id, updatedTerm.Name, updatedTerm.StartDate, updatedTerm.EndDate);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _genericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find Term with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _genericRepository.DeleteById(id);
        }
    }
}
