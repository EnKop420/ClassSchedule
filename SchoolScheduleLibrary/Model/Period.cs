namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class Period
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;

        public Period(string name, TimeOnly startTime, TimeOnly endTime, Guid institutionId)
        {
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
            InstitutionId = institutionId;
        }
    }
}
