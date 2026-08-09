using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record LessonTemplateDTO(Guid Id, int WeekDay, DateOnly ValidFrom, DateOnly ValidTo, string HoldName, string PeriodName, string? RoomName = "");
    public record CreateLessonTemplateDTO(int WeekDay, DateOnly ValidFrom, DateOnly ValidTo, Guid HoldId, Guid PeriodId, Guid? RoomId = null);
    public record UpdateLessonTemplateDTO(Guid Id, int WeekDay, DateOnly ValidFrom, DateOnly ValidTo, Guid HoldId, Guid PeriodId, Guid? RoomId = null);
}
