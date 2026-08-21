namespace SchoolScheduleLibrary.Model
{
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
