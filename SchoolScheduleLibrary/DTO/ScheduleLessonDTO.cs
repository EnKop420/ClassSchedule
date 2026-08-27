using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record ScheduleLessonDTO(
        Guid Id,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string SubjectName,
        string HoldName,
        string? RoomName,
        string Status,
        List<MinimalUserInformationDTO> Teachers);

    public record GetScheduleLessonDTO(Guid TargetId, DateOnly From, DateOnly To);
}
