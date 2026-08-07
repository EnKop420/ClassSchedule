using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record TermDTO(Guid Id, string Name, DateOnly StartDate, DateOnly EndDate);
    public record CreateTermDTO(string Name, DateOnly StartDate, DateOnly EndDate);
}
