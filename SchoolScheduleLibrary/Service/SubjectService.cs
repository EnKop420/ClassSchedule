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
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _subjectGenericRepository;
        public SubjectService(IGenericRepository<Subject> genericRepository)
        {
            _subjectGenericRepository = genericRepository;
        }

        public async Task<List<SubjectDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _subjectGenericRepository.GetAll())
                .Where(s => s.InstitutionId == institutionId)
                .Select(s => new SubjectDTO(s.Id, s.Name)).ToList();
        }

        public async Task<SubjectDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == id && s.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new SubjectDTO(subject.Id, subject.Name);
        }

        public async Task<SubjectDTO> CreateAsync(Guid institutionId, CreateSubjectDTO dto)
        {
            Subject subject = new(dto.Name, institutionId);

            await _subjectGenericRepository.Add(subject);
            return new SubjectDTO(subject.Id, subject.Name);
        }
        public async Task<SubjectDTO> UpdateAsync(Guid institutionId, SubjectDTO dto)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == dto.Id && s.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            subject.Name = dto.Name;

            Subject updatedSubject = await _subjectGenericRepository.Update(subject);

            return new SubjectDTO(updatedSubject.Id, updatedSubject.Name);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _subjectGenericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find Subject with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _subjectGenericRepository.Delete(s => s.Id == id);
        }
    }
}
