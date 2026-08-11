using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record NonTeachingDayDTO(Guid Id, DateOnly StartDate, DateOnly EndDate, string Reason);
    public record CreateNonTeachingDayDTO(DateOnly StartDate, DateOnly EndDate, string Reason);
}
