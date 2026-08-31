using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class AbsenceService : IAbsenceService
    {
        private readonly IGenericRepository<Lesson> _lessonGenericRepository;
        private readonly IGenericRepository<Absence> _absenceGenericRepository;
        private readonly IGenericRepository<Enrollment> _enrollmentGenericRepository;

        public AbsenceService(
            IGenericRepository<Lesson> lessonGenericRepository,
            IGenericRepository<Absence> absenceGenericRepository,
            IGenericRepository<Enrollment> enrollmentGenericRepository
        )
        {
            _lessonGenericRepository = lessonGenericRepository;
            _absenceGenericRepository = absenceGenericRepository;
            _enrollmentGenericRepository = enrollmentGenericRepository;
        }

        public async Task<bool> SetAbsence(Guid lessonId, List<SetAbsenceDTO> dtos, Guid teacherId)
        {
            Lesson lesson = await _lessonGenericRepository.Get(l => l.Id == lessonId, l => l.Absences)
                ?? throw new NotFoundException($"Could not get Lesson with Id \"{lessonId}\"");

            lesson.Absences.Clear();

            if (dtos.Count == 0) return await _lessonGenericRepository.Update(lesson);

            var studentIds = dtos.Select(a => a.StudentId).Distinct().ToList();

            bool hasDuplicateStudents = dtos.Count != studentIds.Count;
            if (hasDuplicateStudents) throw new BadRequestException("There is one or more duplicate student ids!");

            int enrolledStudentCount = await _enrollmentGenericRepository.Count(e =>
                e.HoldId == lesson.HoldId && studentIds.Contains(e.StudentId));

            if (enrolledStudentCount != studentIds.Count)
            {
                throw new BadRequestException("One or more students are not enrolled in this lesson!");
            }

            List<Absence> absences = new();
            foreach (var dto in dtos)
            {
                absences.Add(new Absence(lessonId, dto.StudentId, dto.Status, teacherId));
            }

            if (lesson.IsModified == false && absences.Count > 0) lesson.IsModified = true;

            if (await _absenceGenericRepository.AddRange(absences))
            {
                return await _lessonGenericRepository.Update(lesson);
            }
            else throw new InternalErrorException("Something went wrong when setting the absences on the students!");
        }

        public async Task<List<AbsenceDTO>> GetAllAbsences(Guid lessonId)
        {
            return (await _absenceGenericRepository.GetAll(a => a.LessonId == lessonId))
                .Select(a => new AbsenceDTO(a.Id, a.LessonId, a.StudentId, a.Status, a.RegisteredById))
                .ToList();
        }
    }
}
