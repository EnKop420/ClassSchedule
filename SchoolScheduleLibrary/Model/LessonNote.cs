using System.ComponentModel.DataAnnotations;

namespace SchoolScheduleLibrary.Model
{
    public class LessonNote
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LessonId { get; set; }
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
        public Guid? EditorId { get; set; }
        public User Editor { get; set; } = null!;

        [MinLength(2)]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastEditedAt { get; set; }

        public LessonNote(Guid lessonId, Guid authorId, string content)
        {
            LessonId = lessonId;
            AuthorId = authorId;
            Content = content;
        }
    }
}
