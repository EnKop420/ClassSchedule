using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _genericRepository;
        public SubjectService(IGenericRepository<Subject> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<SubjectDTO> CreateAsync(Guid institutionId, CreateSubjectDTO dto)
        {
            Subject subject = new Subject { Name = dto.Name, InstitutionId = institutionId };
            await _genericRepository.Create(subject);
            return new SubjectDTO(subject.Id, subject.Name);
        }

        public async Task<List<SubjectDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _genericRepository.GetAll())
                .Where(s => s.InstitutionId == institutionId)
                .Select(s => new SubjectDTO(s.Id, s.Name)).ToList();
        }
    }
}
