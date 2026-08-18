using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record PeriodDTO(Guid Id, string Name, TimeOnly StartTime, TimeOnly EndTime);
    public record CreatePeriodDTO(string Name, TimeOnly StartTime, TimeOnly EndTime);
}
