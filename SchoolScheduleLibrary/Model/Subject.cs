namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class Subject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;

        public Subject(string name, Guid institutionId)
        {
            Name = name;
            InstitutionId = institutionId;
        }
    }
}
