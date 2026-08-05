using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Context
{
    // Add-Migration InitDb -Context SchoolDbContext -OutputDir "Migrations/ScheduleMigration"
    // update-database
    public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options)
    {
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Prevents duplicates per institution. Allows duplicates across institutions.
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Institution.Id, u.Email })
                .IsUnique();
        }
    }
}
