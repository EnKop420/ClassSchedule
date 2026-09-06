using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Repository
{
    public class LessonRepository : ILessonRepository
    {
        private readonly SchoolDbContext _context;

        public LessonRepository(SchoolDbContext context)
        {
            _context = context;
        }
        public async Task<List<User>> GetStudentsFromLessonAsync(Guid lessonId)
        {
            return await _context.Lessons
                .Where(l => l.Id == lessonId)
                .SelectMany(l => l.Hold.Enrollments)
                .Select(e => e.Student)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<List<LessonDTO>> GetStudentLessonsAsync(Guid institutionId, GetLessonDTO dto)
        {
            // Gets a list of Lessons and orders it by date and then start time and automatically maps it to a DTO
            return await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Hold.Enrollments.Any(e => e.StudentId == dto.TargetId))
                .OrderBy(l => l.Date)
                .ThenBy(l => l.StartTime)
                .Select(l => new LessonDTO
                (
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    l.Hold.Subject.Name,
                    l.Hold.Name,
                    l.Room != null ? l.Room.Name : string.Empty,
                    l.Status.ToString(),
                    l.Note != null ? new LessonNoteDTO( // Map to the LessonNoteDTO
                        l.Note.Id,
                        l.Note.LessonId,
                        l.Note.AuthorId,
                        l.Note.EditorId,
                        l.Note.Content,
                        l.Note.CreatedAt,
                        l.Note.LastEditedAt
                    ) : null,
                    l.Teachers.Select(t => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                        t.TeacherId
                    )).ToList(),
                    l.Absences.Select(a => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{a.Student.FirstName} {a.Student.LastName}",
                        a.StudentId
                    )).ToList()

                )).ToListAsync();
        }

        public async Task<List<LessonDTO>> GetTeacherLessonsAsync(Guid institutionId, GetLessonDTO dto)
        {
            // Gets a list of Lessons and orders it by date and then start time and automatically maps it to a DTO
            return await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Teachers.Any(e => e.TeacherId == dto.TargetId))
                .OrderBy(l => l.Date)
                .ThenBy(l => l.StartTime)
                .Select(l => new LessonDTO
                (
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    l.Hold.Subject.Name,
                    l.Hold.Name,
                    l.Room != null ? l.Room.Name : string.Empty,
                    l.Status.ToString(),
                    l.Note != null ? new LessonNoteDTO( // Map to the LessonNoteDTO
                        l.Note.Id,
                        l.Note.LessonId,
                        l.Note.AuthorId,
                        l.Note.EditorId,
                        l.Note.Content,
                        l.Note.CreatedAt,
                        l.Note.LastEditedAt
                    ) : null,
                    l.Teachers.Select(t => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                        t.TeacherId
                    )).ToList(),
                    l.Absences.Select(a => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{a.Student.FirstName} {a.Student.LastName}",
                        a.StudentId
                    )).ToList()

                )).ToListAsync();
        }

        public async Task<LessonDTO?> GetLesson(Guid lessonId)
        {
            // Gets a specific lesson and automatically maps it to a DTO
            return await _context.Lessons
                .Where(l => l.Id == lessonId)
                .Select(l => new LessonDTO
                (
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    l.Hold.Subject.Name,
                    l.Hold.Name,
                    l.Room != null ? l.Room.Name : string.Empty,
                    l.Status.ToString(),
                    l.Note != null ? new LessonNoteDTO( // Map to the LessonNoteDTO
                        l.Note.Id,
                        l.Note.LessonId,
                        l.Note.AuthorId,
                        l.Note.EditorId,
                        l.Note.Content,
                        l.Note.CreatedAt,
                        l.Note.LastEditedAt
                    ) : null,
                    l.Teachers.Select(t => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                        t.TeacherId
                    )).ToList(),
                    l.Absences.Select(a => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{a.Student.FirstName} {a.Student.LastName}",
                        a.StudentId
                    )).ToList()

                ))
                .FirstOrDefaultAsync();
        }

        public async Task test(Guid institutionId, GetLessonDTO dto)
        {
            Guid id = Guid.NewGuid();
            List<LessonDTO> list = await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Teachers.Any(e => e.TeacherId == dto.TargetId))
                .OrderBy(l => l.Date)
                .ThenBy(l => l.StartTime)
                .Select(l => new LessonDTO
                (
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    l.Hold.Subject.Name,
                    l.Hold.Name,
                    l.Room != null ? l.Room.Name : string.Empty,
                    l.Status.ToString(),
                    l.Note != null ? new LessonNoteDTO( // Map to the LessonNoteDTO
                        l.Note.Id,
                        l.Note.LessonId,
                        l.Note.AuthorId,
                        l.Note.EditorId,
                        l.Note.Content,
                        l.Note.CreatedAt,
                        l.Note.LastEditedAt
                    ) : null,
                    l.Teachers.Select(t => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                        t.TeacherId
                    )).ToList(),
                    l.Absences.Select(a => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{a.Student.FirstName} {a.Student.LastName}",
                        a.StudentId
                    )).ToList()

                )).ToListAsync();
        }
    }
}
