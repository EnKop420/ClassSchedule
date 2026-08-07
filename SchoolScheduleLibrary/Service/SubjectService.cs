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
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _genericRepository;
        public SubjectService(IGenericRepository<Subject> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<List<SubjectDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _genericRepository.GetAll())
                .Where(s => s.InstitutionId == institutionId)
                .Select(s => new SubjectDTO(s.Id, s.Name)).ToList();
        }

        public async Task<SubjectDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Subject subject = await _genericRepository.GetById(id) ?? throw new NotFoundException($"Subject with ID {id} does not exist.");
            if (subject.InstitutionId != institutionId) throw new BadRequestException("Subject is not apart of the Institution!");

            return new SubjectDTO(subject.Id, subject.Name);
        }

        public async Task<SubjectDTO> CreateAsync(Guid institutionId, CreateSubjectDTO dto)
        {
            Subject subject = new Subject { Name = dto.Name, InstitutionId = institutionId };
            await _genericRepository.Create(subject);
            return new SubjectDTO(subject.Id, subject.Name);
        }
        public async Task<SubjectDTO> UpdateAsync(Guid institutionId, SubjectDTO dto)
        {
            Subject subject = await _genericRepository.GetById(dto.Id) ?? throw new NotFoundException($"Subject with ID {dto.Id} does not exist.");
            if (subject.InstitutionId != institutionId) throw new BadRequestException("Subject is not apart of the Institution!");

            subject.Name = dto.Name;

            Subject updatedSubject = await _genericRepository.Update(subject);

            return new SubjectDTO(updatedSubject.Id, updatedSubject.Name);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            Subject subject = await _genericRepository.GetById(id) ?? throw new NotFoundException($"Subject with ID {id} does not exist.");
            if (subject.InstitutionId != institutionId) throw new BadRequestException("Subject is not apart of the Institution!");

            return await _genericRepository.Delete(subject);
        }
    }
}
