using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record LessonNoteDTO(Guid Id, Guid LessonId, Guid AuthorId, string Content, DateTime CreatedAt);
    public record CreateLessonNoteDTO(Guid LessonId, Guid AuthorId, string Content);
    public record UpdateLessonNoteDTO(Guid Id, Guid EditorId, string Content);
}
