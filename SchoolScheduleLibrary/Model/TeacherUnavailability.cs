using SchoolScheduleLibrary.Enums;

namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class TeacherUnavailability
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
