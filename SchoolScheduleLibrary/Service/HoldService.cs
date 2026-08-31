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
            // Get all holds
            List<Hold> holds = await _holdGenericRepository.GetAll(
                h => h.InstitutionId == institutionId, // Predicate
                h => h.Subject, // Include
                h => h.Term, // Include
                h => h.Enrollments, // Include
                h => h.GroupTeachers // Include
            );

            List<HoldDTO> dtoList = new();

            // Get students and teachers sequentially.
            foreach (var hold in holds)
            {
                dtoList.Add(new HoldDTO(
                    hold.Id,
                    hold.Name,
                    hold.SubjectId,
                    hold.TermId,
                    hold.Subject.Name,
                    hold.Term.Name,
                    hold.GroupTeachers.Select(t => t.TeacherId).ToList(),
                    hold.Enrollments.Select(s => s.StudentId).ToList()
                ));
            }

            return dtoList;
        }

        public async Task<HoldDTO> GetByIdAsync(Guid id)
        {
            Hold hold = await _holdGenericRepository.Get(
                h => h.Id == id, // Predicate
                h => h.Subject, // Include
                h => h.Term, // Include
                h => h.Enrollments, // Include
                h => h.GroupTeachers // Include
            )
            ?? throw new NotFoundException($"Could not get Hold with Id \"{id}\"");

            return new HoldDTO(
                hold.Id,
                hold.Name,
                hold.SubjectId,
                hold.TermId,
                hold.Subject.Name,
                hold.Term.Name,
                hold.GroupTeachers.Select(t => t.TeacherId).ToList(),
                hold.Enrollments.Select(s => s.StudentId).ToList()
            );
        }

        public async Task<bool> CreateAsync(Guid institutionId, CreateHoldDTO dto)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == dto.SubjectId)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{dto.SubjectId}\"");

            Term term = await _termGenericRepository.Get(t => t.Id == dto.TermId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{dto.TermId}\"");

            Hold hold = new(dto.Name, institutionId, dto.SubjectId, dto.TermId);

            hold.Enrollments.Clear();
            hold.GroupTeachers.Clear();

            hold.Enrollments = dto.Students.Distinct()
                .Select(s => new Enrollment(hold.Id, s))
                .ToList();
            hold.GroupTeachers = dto.Teachers.Distinct()
                .Select(t => new GroupTeacher(hold.Id, t))
                .ToList();

            if (await _holdGenericRepository.Add(hold))
            {
                return true;
            }
            else throw new InternalErrorException("Something went wrong while adding the Hold to the database");
        }

        public async Task<bool> UpdateAsync(HoldDTO dto)
        {
            Hold hold = await _holdGenericRepository.Get(h => h.Id == dto.Id, h => h.Enrollments, h => h.GroupTeachers)
                ?? throw new NotFoundException($"Could not get Hold with Id \"{dto.Id}\"");

            // Check subject and terms are valid.
            if (!await _subjectGenericRepository.DoesValueExist(s => s.Id == dto.SubjectId))
                throw new NotFoundException($"Could not find Subject with Id \"{dto.SubjectId}\"");

            if (!await _termGenericRepository.DoesValueExist(t => t.Id == dto.TermId))
                throw new NotFoundException($"Could not find Term with Id \"{dto.TermId}\"");

            hold.Name = dto.Name;
            hold.SubjectId = dto.SubjectId;
            hold.TermId = dto.TermId;

            hold.Enrollments.Clear();
            hold.GroupTeachers.Clear();

            hold.Enrollments = dto.Students.Distinct()
                .Select(s => new Enrollment(hold.Id, s))
                .ToList();
            hold.GroupTeachers = dto.Teachers.Distinct()
                .Select(t => new GroupTeacher(hold.Id, t))
                .ToList();

            return await _holdGenericRepository.Update(hold);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _holdGenericRepository.DoesValueExist(t => t.Id == id))
            {
                throw new NotFoundException($"Could not find Hold with Id \"{id}\"");
            }

            return await _holdGenericRepository.Delete(h => h.Id == id);
        }
    }
}
