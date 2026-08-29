using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository
{
    public class LessonRepository : ILessonRepository
    {
        private readonly SchoolDbContext _context;

        public LessonRepository(SchoolDbContext context)
        {
            _context = context;
        }
        public async Task<List<User>> GetStudentsFromLessonAsync(Guid institutionId, Guid lessonId)
        {
            // Get students with the following LINQ:
            // 1. Filter: Match the institution and lesson id
            // 2. SelectMany: Selects the many to many table Enrollments
            // 3. Select: Selects the students.
            // 4. Order by the First name and then Last name
            return await _context.Lessons
                .Where(l => l.Id == lessonId && l.InstitutionId == institutionId)
                .SelectMany(l => l.Hold.Enrollments)
                .Select(e => e.Student)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<List<LessonDTO>> GetStudentLessonsAsync(Guid institutionId, GetLessonDTO dto)
        {
            // Get lessons with the following LINQ pipeline:
            // 1. Filter: Match institution, fall within date range, and ensure target student is assigned to the lesson.
            // 2. Order: Sort chronologically by date, then start time.
            // 3. Project: Select database fields into an anonymous object.
            // 4. Transform: Map anonymous objects in memory to ScheduleLessonDTOs, safely formatting enums and strings.
            return await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Hold.Enrollments.Any(e => e.StudentId == dto.TargetId))
                .OrderBy(l => l.Date)
                .ThenBy(l => l.StartTime)
                .Select(l => new
                {
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    SubjectName = l.Hold.Subject.Name,
                    HoldName = l.Hold.Name,
                    RoomName = l.Room != null ? l.Room.Name : string.Empty,
                    l.Status,
                    Note = l.Note != null ? new LessonNoteDTO( // Map to the LessonNoteDTO
                        l.Note.Id,
                        l.Note.LessonId,
                        l.Note.AuthorId,
                        l.Note.Content,
                        l.Note.CreatedAt
                    ) : null,
                    Teachers = l.Teachers.Select(t => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                        t.TeacherId
                    )).ToList(),
                    Absences = l.Absences.Select(a => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{a.Student.FirstName} {a.Student.LastName}",
                        a.StudentId
                    )).ToList()
                })
                .ToListAsync()
                .ContinueWith(task => task.Result.Select(l => new LessonDTO( // Map to the LessonDTO
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    l.SubjectName,
                    l.HoldName,
                    l.RoomName,
                    l.Status.ToString(),
                    l.Note,
                    l.Teachers,
                    l.Absences
                )).ToList());
        }

        public async Task<List<LessonDTO>> GetTeacherLessonsAsync(Guid institutionId, GetLessonDTO dto)
        {
            // Get lessons with the following LINQ pipeline:
            // 1. Filter: Match institution, fall within date range, and ensure target teacher is assigned to the lesson.
            // 2. Order: Sort chronologically by date, then start time.
            // 3. Project: Select database fields into an anonymous object.
            // 4. Transform: Map anonymous objects in memory to ScheduleLessonDTOs, safely formatting enums and strings.
            return await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Teachers.Any(e => e.TeacherId == dto.TargetId))
                .OrderBy(l => l.Date)
                .ThenBy(l => l.StartTime)
                .Select(l => new
                {
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    SubjectName = l.Hold.Subject.Name,
                    HoldName = l.Hold.Name,
                    RoomName = l.Room != null ? l.Room.Name : string.Empty,
                    l.Status,
                    Note = l.Note != null ? new LessonNoteDTO( // Map to the LessonNoteDTO
                        l.Note.Id,
                        l.Note.LessonId,
                        l.Note.AuthorId,
                        l.Note.Content,
                        l.Note.CreatedAt
                    ) : null,
                    Teachers = l.Teachers.Select(t => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                        t.TeacherId
                    )).ToList(),
                    Absences = l.Absences.Select(a => new MinimalUserInformationDTO( // Map to the MinimalUserInformationDTO
                        $"{a.Student.FirstName} {a.Student.LastName}",
                        a.StudentId
                    )).ToList()
                })
                .ToListAsync()
                .ContinueWith(task => task.Result.Select(l => new LessonDTO( // Map to the LessonDTO
                    l.Id,
                    l.Date,
                    l.StartTime,
                    l.EndTime,
                    l.SubjectName,
                    l.HoldName,
                    l.RoomName,
                    l.Status.ToString(),
                    l.Note,
                    l.Teachers,
                    l.Absences
                )).ToList());
        }
    }
}
