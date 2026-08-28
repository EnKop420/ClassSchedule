namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class Enrollment
    {
        public Guid HoldId { get; set; }
        public Hold Hold { get; set; } = null!;

        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        public Enrollment(Guid holdId, Guid studentId)
        {
            HoldId = holdId;
            StudentId = studentId;
        }
    }
}
