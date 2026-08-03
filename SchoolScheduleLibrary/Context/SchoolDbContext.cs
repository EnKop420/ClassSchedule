using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Context
{
    // Add-Migration InitDb -Context ScheduleDbContext -OutputDir "Migrations/ScheduleMigration"
    public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options)
    {
        public DbSet<Admin> Admin {  get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Classroom>()
                .HasMany(c => c.Students)
                .WithOne(s => s.Classroom)
                .HasForeignKey(s => s.ClassroomId);
        }
    }
}
