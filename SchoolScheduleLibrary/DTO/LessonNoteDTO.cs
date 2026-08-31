using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record LessonNoteDTO(Guid Id, Guid LessonId, Guid AuthorId, Guid? EditorId, string Content, DateTime CreatedAt, DateTime? EditedAt);
    public record CreateLessonNoteDTO(Guid LessonId, string Content);
    public record UpdateLessonNoteDTO(Guid Id, string Content);
}
