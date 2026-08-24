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
        private readonly IHoldMemberService _holdMemberService;
        private readonly IGenericRepository<Hold> _holdGenericRepository;
        private readonly IGenericRepository<Term> _termGenericRepository;
        private readonly IGenericRepository<Subject> _subjectGenericRepository;
        public HoldService(
            IHoldMemberService holdMemberService,
            IGenericRepository<Hold> genericRepository,
            IGenericRepository<Term> termGenericRepository,
            IGenericRepository<Subject> subjectGenericRepository)
        {
            _holdMemberService = holdMemberService;
            _holdGenericRepository = genericRepository;
            _termGenericRepository = termGenericRepository;
            _subjectGenericRepository = subjectGenericRepository;
        }

        public async Task<List<HoldDTO>> GetAllAsync(Guid institutionId)
        {
            // Get all holds
            List<Hold> holds = await _holdGenericRepository.GetAll(
                h => h.InstitutionId == institutionId,
                h => h.Subject,
                h => h.Term
            );

            List<HoldDTO> dtoList = new();

            // Get students and teachers sequentially.
            foreach (var hold in holds)
            {
                List<Guid> students = (await _holdMemberService.GetStudentsAsync(hold.Id)).Select(s => s.UserId).ToList();
                List<Guid> teachers = (await _holdMemberService.GetTeachersAsync(hold.Id)).Select(t => t.UserId).ToList();

                dtoList.Add(new HoldDTO(
                    hold.Id,
                    hold.Name,
                    hold.SubjectId,
                    hold.TermId,
                    hold.Subject.Name,
                    hold.Term.Name,
                    teachers,
                    students
                ));
            }

            return dtoList;
        }

        public async Task<HoldDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            Hold hold = await _holdGenericRepository.Get(
                h => h.Id == id && h.InstitutionId == institutionId, // Predicate
                h => h.Subject, // Include
                h => h.Term // Include
            )
            ?? throw new NotFoundException($"Could not get Hold with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            List<Guid> students = (await _holdMemberService.GetStudentsAsync(hold.Id)).Select(s => s.UserId).ToList();
            List<Guid> teachers = (await _holdMemberService.GetTeachersAsync(hold.Id)).Select(t => t.UserId).ToList();

            return new HoldDTO(hold.Id, hold.Name, hold.SubjectId, hold.TermId, hold.Subject.Name, hold.Term.Name, teachers, students);
        }

        public async Task<HoldDTO> CreateAsync(Guid institutionId, CreateHoldDTO dto)
        {
            Subject subject = await _subjectGenericRepository.Get(s => s.Id == dto.SubjectId && s.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Subject with Id \"{dto.SubjectId}\" in the Institution with Id \"{institutionId}\"");

            Term term = await _termGenericRepository.Get(t => t.Id == dto.TermId && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{dto.TermId}\" in the Institution with Id \"{institutionId}\"");

            Hold hold = new(dto.Name, institutionId, dto.SubjectId, dto.TermId);

            List<Guid> students = dto.Students.Distinct().ToList();
            List<Guid> teachers = dto.Teachers.Distinct().ToList();

            if (await _holdGenericRepository.Add(hold))
            {
                await _holdMemberService.EnrollStudentAsync(institutionId, hold.Id, students);
                await _holdMemberService.GroupTeacherAsync(institutionId, hold.Id, teachers);
            }

            return new HoldDTO(hold.Id, hold.Name, subject.Id, term.Id, subject.Name, term.Name, teachers, students);
        }
        public async Task<HoldDTO> UpdateAsync(Guid institutionId, HoldDTO dto)
        {
            Hold hold = await _holdGenericRepository.Get(h => h.Id == dto.Id && h.InstitutionId == institutionId, h => h.Enrollments, h => h.GroupTeachers)
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

            // Delete the old students and teachers.
            List<Guid> currentStudents = hold.Enrollments.Select(e => e.StudentId).ToList();
            List<Guid> currentTeachers = hold.GroupTeachers.Select(t => t.TeacherId).ToList();

            await _holdMemberService.UnenrollStudentAsync(institutionId, dto.Id, currentStudents);
            await _holdMemberService.UngroupTeacherAsync(institutionId, dto.Id, currentTeachers);

            // Apply the new list instead.
            List<Guid> dtoStudents = dto.Students.Distinct().ToList();
            List<Guid> dtoTeachers = dto.Teachers.Distinct().ToList();

            await _holdMemberService.EnrollStudentAsync(institutionId, dto.Id, dtoStudents);
            await _holdMemberService.GroupTeacherAsync(institutionId, dto.Id, dtoTeachers);

            Hold updatedHold = await _holdGenericRepository.Get(
                h => h.Id == dto.Id && h.InstitutionId == institutionId, // Predicate
                h => h.Subject, // Include
                h => h.Term // Include
            ) ?? throw new InternalErrorException("Something went wrong after updating and could not retrieve it!");

            return new HoldDTO(updatedHold.Id, updatedHold.Name, updatedHold.SubjectId, updatedHold.TermId, updatedHold.Subject.Name, updatedHold.Term.Name, dtoTeachers, dtoStudents);
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
