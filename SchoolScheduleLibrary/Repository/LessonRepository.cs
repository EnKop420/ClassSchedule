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

        public async Task<List<Lesson>> GetStudentLessonsAsync(Guid institutionId, GetScheduleLessonDTO dto)
        {
            // Get lessons with the following LINQ:
            // 1. Filter: Match institution, fall within date range, and ensure target student is enrolled in the hold.
            // 2. Include: Eagerly load related entities (Hold -> Subject, Room, Teachers -> Teacher).
            // 3. Order: Sort chronologically by date, then start time.
            return await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Hold.Enrollments.Any(e => e.StudentId == dto.TargetId))
                .Include(l => l.Hold).ThenInclude(h => h.Subject)
                .Include(l => l.Room)
                .Include(l => l.Teachers).ThenInclude(t => t.Teacher)
                .OrderBy(l => l.Date).ThenBy(l => l.StartTime)
                .ToListAsync();
        }

        public async Task<List<Lesson>> GetTeacherLessonsAsync(Guid institutionId, GetScheduleLessonDTO dto)
        {
            // Get lessons with the following LINQ:
            // 1. Filter: Match institution, fall within date range, and ensure target teacher is in the hold.
            // 2. Include: Eagerly load related entities (Hold -> Subject, Room, Teachers -> Teacher).
            // 3. Order: Sort chronologically by date, then start time.
            return await _context.Lessons
                .Where(l =>
                    l.InstitutionId == institutionId
                    && l.Date >= dto.From && l.Date <= dto.To
                    && l.Teachers.Any(t => t.TeacherId == dto.TargetId))
                .Include(l => l.Hold).ThenInclude(h => h.Subject)
                .Include(l => l.Room)
                .Include(l => l.Teachers).ThenInclude(t => t.Teacher)
                .OrderBy(l => l.Date).ThenBy(l => l.StartTime)
                .ToListAsync();
        }
    }
}
