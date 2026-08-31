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

            var studentIds = dtos.Select(a => a.StudentId).ToList();

            int enrolledStudentCount = await _enrollmentGenericRepository.Count(e =>
                e.HoldId == lesson.HoldId && studentIds.Contains(e.StudentId));

            if (enrolledStudentCount != studentIds.Count)
            {
                throw new BadRequestException("One or more students are not enrolled in this lesson!");
            }

            foreach (var dto in dtos)
            {
                lesson.Absences.Add(new Absence(lessonId, dto.StudentId, dto.Status, teacherId));
            }

            if (lesson.IsModified == false && lesson.Absences.Count > 0) lesson.IsModified = true;

            return await _lessonGenericRepository.Update(lesson);
        }

        public async Task<List<AbsenceDTO>> GetAllAbsences(Guid lessonId)
        {
            return (await _absenceGenericRepository.GetAll(a => a.LessonId == lessonId))
                .Select(a => new AbsenceDTO(a.Id, a.LessonId, a.StudentId, a.Status, a.RegisteredById))
                .ToList();
        }
    }
}
