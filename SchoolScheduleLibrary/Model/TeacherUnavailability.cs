using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    public class TeacherUnavailability : IBaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;
        public DateOnly Date { get; set; }
        public TimeOnly? StartTime { get; set; } // null = whole day
        public TimeOnly? EndTime { get; set; }
        public string? Reason { get; set; }
        public UnavailabilityStatus Status { get; set; } = UnavailabilityStatus.Requested;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
