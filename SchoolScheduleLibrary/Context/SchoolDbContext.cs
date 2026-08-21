using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SchoolScheduleLibrary.Context
{
    // Add-Migration InitDb -Context SchoolDbContext -OutputDir "Migrations/ProdScheduleMigration"
    // update-database
    public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options)
    {
        // "=> Set<T>()" makes it a readonly and is the new way EF Core does it.
        public DbSet<Institution> Institutions => Set<Institution>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Term> Terms => Set<Term>();
        public DbSet<Period> Periods => Set<Period>();
        public DbSet<NonTeachingDay> NonTeachingDays => Set<NonTeachingDay>();
        public DbSet<Hold> Holds => Set<Hold>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<GroupTeacher> GroupTeachers => Set<GroupTeacher>();
        public DbSet<LessonTemplate> LessonTemplates => Set<LessonTemplate>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<LessonTeacher> LessonTeachers => Set<LessonTeacher>();
        public DbSet<TeacherUnavailability> TeacherUnavailabilities => Set<TeacherUnavailability>();
        public DbSet<LessonNote> LessonNotes => Set<LessonNote>();
        public DbSet<Absence> Absences => Set<Absence>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderUtility.ApplyModelBuilder(modelBuilder);
        }
    }
}
