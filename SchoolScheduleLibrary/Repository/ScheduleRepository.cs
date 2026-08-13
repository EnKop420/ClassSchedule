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
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly SchoolDbContext _context;

        public ScheduleRepository(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<List<Lesson>> GetStudentLessonsAsync(Guid institutionId, GetScheduleLessonDTO dto)
        {
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
