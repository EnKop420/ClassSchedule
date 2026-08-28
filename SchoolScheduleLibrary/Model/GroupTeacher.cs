namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class GroupTeacher
    {
        public Guid HoldId { get; set; }
        public Hold Hold { get; set; } = null!;

        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public GroupTeacher(Guid holdId, Guid teacherId)
        {
            HoldId = holdId;
            TeacherId = teacherId;
        }
    }
}
