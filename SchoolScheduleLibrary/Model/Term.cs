namespace SchoolScheduleLibrary.Model
{
    public class Term
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;

        public Term(string name, DateOnly startDate, DateOnly endDate, Guid institutionId)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            InstitutionId = institutionId;
        }
    }
}
