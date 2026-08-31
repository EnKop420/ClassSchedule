using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record LessonDTO(
        Guid Id,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string SubjectName,
        string HoldName,
        string? RoomName,
        string Status,
        LessonNoteDTO? Note,
        List<MinimalUserInformationDTO> Teachers,
        List<MinimalUserInformationDTO> AbsentStudents);
    public record GetLessonDTO(Guid TargetId, DateOnly From, DateOnly To);
}
