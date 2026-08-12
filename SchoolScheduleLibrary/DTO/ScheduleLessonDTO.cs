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
        string? RoomName,
        string Status,
        List<string> Teachers);


}
