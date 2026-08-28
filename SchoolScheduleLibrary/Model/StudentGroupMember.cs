namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class StudentGroupMember
    {
        public Guid StudentGroupId { get; set; }
        public StudentGroup StudentGroup { get; set; } = null!;

        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        public StudentGroupMember(Guid studentGroupId, Guid studentId)
        {
            StudentGroupId = studentGroupId;
            StudentId = studentId;
        }
    }
}
