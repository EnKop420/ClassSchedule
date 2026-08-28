namespace SchoolScheduleLibrary.Model
{
    public class LessonNote
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
