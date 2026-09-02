using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record AbsenceDTO(Guid Id, Guid LessonId, Guid StudentIds, AbsenceStatus Status, Guid RegisteredById);
    public record AbsenceWithLessonDTO(Guid Id, Guid LessonId, AbsenceStatus Status, Guid RegisteredById);
    public record SetAbsenceDTO(Guid StudentId, AbsenceStatus Status);
}
