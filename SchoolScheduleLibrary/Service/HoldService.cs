using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class HoldService : IHoldService
    {
        private readonly IGenericRepository<Hold> _holdGenericRepository;
        private readonly IGenericRepository<Term> _termGenericRepository;
        private readonly IGenericRepository<Subject> _subjectGenericRepository;
        public HoldService(
            IGenericRepository<Hold> genericRepository,
            IGenericRepository<Term> termGenericRepository,
            IGenericRepository<Subject> subjectGenericRepository)
        {
            _holdGenericRepository = genericRepository;
            _termGenericRepository = termGenericRepository;
            _subjectGenericRepository = subjectGenericRepository;
        }

        public async Task<List<HoldDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _holdGenericRepository.GetAll(
                h => h.InstitutionId == institutionId, // Predicate
                h => h.Subject, // Include
                h => h.Term // Include
                )
            )
            .Select(h => new HoldDTO(h.Id, h.Name, h.Subject.Name, h.Term.Name)).ToList();
        }

        public async Task<HoldDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Hold hold = await _holdGenericRepository.Get(
                h => h.Id == id && h.InstitutionId == institutionId, // Predicate
                h => h.Subject, // Include
                h => h.Term // Include
            )
            ?? throw new NotFoundException($"Could not get Hold with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new HoldDTO(hold.Id, hold.Name, hold.Subject.Name, hold.Term.Name);
        }

        public async Task<HoldDTO> CreateAsync(Guid institutionId, CreateHoldDTO dto)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == dto.SubjectId && s.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{dto.SubjectId}\" in the Institution with Id \"{institutionId}\"");

            Term term = await _termGenericRepository.Get(t => t.Id == dto.TermId && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{dto.TermId}\" in the Institution with Id \"{institutionId}\"");

            Hold hold = new(dto.Name, institutionId, dto.TermId, dto.SubjectId);

            await _holdGenericRepository.Add(hold);
            return new HoldDTO(hold.Id, hold.Name, subject.Name, term.Name);
        }
        public async Task<HoldDTO> UpdateAsync(Guid institutionId, UpdateHoldDTO dto)
        {
            Hold hold = await _holdGenericRepository.Get(h => h.Id == dto.Id && h.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Hold with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            // Check subject and terms are valid.
            if (!await _subjectGenericRepository.DoesValueExist(s => s.InstitutionId == institutionId && s.Id == dto.SubjectId))
                throw new NotFoundException($"Could not find Subject with Id \"{dto.SubjectId}\" in the Institution with Id \"{institutionId}\"");

            if (!await _termGenericRepository.DoesValueExist(t => t.InstitutionId == institutionId && t.Id == dto.TermId))
                throw new NotFoundException($"Could not find Term with Id \"{dto.TermId}\" in the Institution with Id \"{institutionId}\"");

            hold.Name = dto.Name;
            hold.SubjectId = dto.SubjectId;
            hold.TermId = dto.TermId;

            await _holdGenericRepository.Update(hold);

            Hold updatedHold = await _holdGenericRepository.Get(
                h => h.Id == dto.Id && h.InstitutionId == institutionId, // Predicate
                h => h.Subject, // Include
                h => h.Term // Include
            ) ?? throw new InternalErrorException("Something went wrong after updating and could not retrieve it!");

            return new HoldDTO(updatedHold.Id, updatedHold.Name, updatedHold.Subject.Name, updatedHold.Term.Name);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _holdGenericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find Hold with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _holdGenericRepository.Delete(h => h.Id == id);
        }
    }
}
