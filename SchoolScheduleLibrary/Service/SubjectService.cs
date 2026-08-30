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

        public async Task<SubjectDTO> GetByIdAsync(Guid id)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == id)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{id}\"");

            return new SubjectDTO(subject.Id, subject.Name);
        }

        public async Task<bool> CreateAsync(Guid institutionId, CreateSubjectDTO dto)
        {
            Subject subject = new(dto.Name, institutionId);

            return await _subjectGenericRepository.Add(subject);
        }
        public async Task<bool> UpdateAsync(SubjectDTO dto)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == dto.Id)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{dto.Id}\"");

            subject.Name = dto.Name;

            return await _subjectGenericRepository.Update(subject);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _subjectGenericRepository.DoesValueExist(t => t.Id == id))
            {
                throw new NotFoundException($"Could not find Subject with Id \"{id}\"");
            }

            return await _subjectGenericRepository.Delete(s => s.Id == id);
        }
    }
}
