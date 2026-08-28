namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class StudentGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;
        public List<StudentGroupMember> Students { get; set; } = new();

        public StudentGroup(string name, Guid institutionId)
        {
            Name = name;
            InstitutionId = institutionId;
        }
    }
}
