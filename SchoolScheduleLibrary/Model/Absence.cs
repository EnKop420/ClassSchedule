using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    public class Absence : IBaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;

        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        public AttendanceStatus Status { get; set; }

        public Guid? RegisteredById { get; set; } // second FK to User
        public User? RegisteredBy { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    }
}
