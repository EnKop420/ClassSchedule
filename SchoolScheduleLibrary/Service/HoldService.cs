using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository;
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
        private readonly IHoldRepository _holdRepository;
        public HoldService(
            IGenericRepository<Hold> genericRepository,
            IGenericRepository<Term> termGenericRepository,
            IGenericRepository<Subject> subjectGenericRepository,
            IHoldRepository holdRepository
        )
        {
            _holdGenericRepository = genericRepository;
            _termGenericRepository = termGenericRepository;
            _subjectGenericRepository = subjectGenericRepository;
            _holdRepository = holdRepository;
        }

        public async Task<List<HoldDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _holdGenericRepository.GetAll())
                .Where(h => h.InstitutionId == institutionId)
                .Select(h => new HoldDTO(h.Id, h.Name, h.Subject.Name, h.Term.Name)).ToList();
        }

        public async Task<HoldDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Hold hold = await _holdRepository.GetById(institutionId, id) ?? throw new NotFoundException($"Hold with ID {id} does not exist in the Institution with the Id {institutionId}");

            return new HoldDTO(hold.Id, hold.Name, hold.Subject.Name, hold.Term.Name);
        }

        public async Task<HoldDTO> CreateAsync(Guid institutionId, CreateHoldDTO dto)
        {
            Subject subject = await _subjectGenericRepository.GetById(dto.subjectId) ?? throw new NotFoundException($"Subject with ID {dto.subjectId} does not exist.");
            if (subject.InstitutionId != institutionId) throw new BadRequestException("Subject is not apart of the Institution!");

            Term term = await _termGenericRepository.GetById(dto.termId) ?? throw new NotFoundException($"Term with ID {dto.termId} does not exist.");
            if (term.InstitutionId != institutionId) throw new BadRequestException("Term is not apart of the Institution");
                
            Hold hold = new Hold
            {
                InstitutionId = institutionId,
                SubjectId = dto.subjectId,
                TermId = dto.termId,
                Name = dto.Name
            };

            await _holdGenericRepository.Create(hold);
            return new HoldDTO(hold.Id, hold.Name, subject.Name, term.Name);
        }
        public async Task<HoldDTO> UpdateAsync(Guid institutionId, UpdateHoldDTO dto)
        {
            Hold hold = await _holdGenericRepository.GetById(dto.Id) ?? throw new NotFoundException($"Hold with ID {dto.Id} does not exist.");
            if (hold.InstitutionId != institutionId) throw new BadRequestException("Hold is not apart of the Institution!");

            // Check subject and terms are valid. This function will throw an error the controller catches if something fails.
            await _holdRepository.CheckTermAndSubjectForInstitution(institutionId, dto.subjectId, dto.termId);

            hold.Name = dto.Name;
            hold.SubjectId = dto.subjectId;
            hold.TermId = dto.termId;

            Hold updatedHold = await _holdRepository.Update(hold);

            return new HoldDTO(updatedHold.Id, updatedHold.Name, updatedHold.Subject.Name, updatedHold.Term.Name);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            Hold hold = await _holdGenericRepository.GetById(id) ?? throw new NotFoundException($"Hold with ID {id} does not exist.");
            if (hold.InstitutionId != institutionId) throw new BadRequestException("Hold is not apart of the Institution!");

            return await _holdGenericRepository.Delete(hold);
        }
    }
}
