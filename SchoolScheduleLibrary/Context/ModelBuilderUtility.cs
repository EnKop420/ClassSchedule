using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Context
{
    internal static class ModelBuilderUtility
    {
        public static void ApplyModelBuilder(ModelBuilder modelBuilder)
        {
            EnumModelBuilder(modelBuilder);
            ManyToMany(modelBuilder);
            UniqueIndexes(modelBuilder);
            SimpleIndex(modelBuilder);
            DefaultValue(modelBuilder);
            ForeignKeyLink(modelBuilder);
            DeleteBehaviorOption(modelBuilder);
        }

        // Automatically converts the Enums to string values when it enters the database and back into enums when it is extracted.
        private static void EnumModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
            modelBuilder.Entity<Lesson>().Property(l => l.Status).HasConversion<string>();
            modelBuilder.Entity<LessonTeacher>().Property(lt => lt.Role).HasConversion<string>();
            modelBuilder.Entity<Absence>().Property(a => a.Status).HasConversion<string>();
            modelBuilder.Entity<TeacherUnavailability>().Property(t => t.Status).HasConversion<string>();
        }

        // Combines two IDs into a single key to prevent duplicate relationships.
        private static void ManyToMany(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>().HasKey(e => new { e.HoldId, e.StudentId });
            modelBuilder.Entity<GroupTeacher>().HasKey(g => new { g.HoldId, g.TeacherId });
            modelBuilder.Entity<LessonTeacher>().HasKey(lt => new { lt.LessonId, lt.TeacherId });
        }

        // Prevents duplicates in the same group. Allows duplicates across different groups.
        private static void UniqueIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.InstitutionId, u.Email }).IsUnique();

            modelBuilder.Entity<NonTeachingDay>()
                .HasIndex(n => new { n.InstitutionId, n.Date }).IsUnique();

            modelBuilder.Entity<Lesson>()
                .HasIndex(l => new { l.TemplateId, l.Date }).IsUnique();

            modelBuilder.Entity<Absence>()
                .HasIndex(a => new { a.LessonId, a.StudentId }).IsUnique();

        }

        // Simple indexing for quick lookup for values that will be searched / filtered on alot.
        private static void SimpleIndex(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lesson>().HasIndex(l => new { l.HoldId, l.Date });
            modelBuilder.Entity<Lesson>().HasIndex(l => new { l.InstitutionId, l.Date });
            modelBuilder.Entity<LessonTeacher>().HasIndex(lt => lt.TeacherId);
            modelBuilder.Entity<Absence>().HasIndex(a => a.StudentId);
            modelBuilder.Entity<Enrollment>().HasIndex(e => e.StudentId);
            modelBuilder.Entity<GroupTeacher>().HasIndex(g => g.TeacherId);
        }

        // Sets default values.
        private static void DefaultValue(ModelBuilder modelBuilder) => modelBuilder.Entity<Lesson>().Property(l => l.IsModified).HasDefaultValue(false);

        // // Configures two different links (Student and RegisteredBy) that both point to the User table.
        private static void ForeignKeyLink(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.RegisteredBy)
                .WithMany()
                .HasForeignKey(a => a.RegisteredById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // Sets the DeleteBehavior to restrict so values can't be deleted before another value is deleted.
        private static void DeleteBehaviorOption(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student).WithMany()
                .HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupTeacher>()
                .HasOne(g => g.Teacher).WithMany()
                .HasForeignKey(g => g.TeacherId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonTeacher>()
                .HasOne(lt => lt.Teacher).WithMany()
                .HasForeignKey(lt => lt.TeacherId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hold>()
                .HasOne(h => h.Subject).WithMany()
                .HasForeignKey(h => h.SubjectId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hold>()
                .HasOne(h => h.Term).WithMany()
                .HasForeignKey(h => h.TermId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonTemplate>()
                .HasOne(lt => lt.Room).WithMany()
                .HasForeignKey(lt => lt.RoomId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonTemplate>()
                .HasOne(lt => lt.Period).WithMany()
                .HasForeignKey(lt => lt.PeriodId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson>()
                .HasOne(lt => lt.Room).WithMany()
                .HasForeignKey(lt => lt.RoomId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
