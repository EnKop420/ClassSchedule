namespace SchoolScheduleLibrary.Model
{
    public class Institution
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Institution(string name)
        {
            Name = name;
        }
    }
}
