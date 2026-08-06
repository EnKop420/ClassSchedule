using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class Hold : IBaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;

        public Guid TermId { get; set; }
        public Term Term { get; set; } = null!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        // Used for queries and the generator
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<GroupTeacher> GroupTeachers { get; set; } = new();
        public List<Lesson> Lessons { get; set; } = new();
        public List<LessonTemplate> LessonTemplates { get; set; } = new();
    }
}
