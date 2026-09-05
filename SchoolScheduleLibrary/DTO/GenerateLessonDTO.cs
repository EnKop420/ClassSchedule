using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record GenerateLessonDTO(Guid termId, List<Guid> LessonTemplateIds);
    public record DeleteLessonDTO(List<Guid> LessonTemplateIds);
}
