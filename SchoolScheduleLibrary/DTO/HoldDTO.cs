using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record HoldDTO(Guid Id, string Name, string SubjectName, string TermName);
    public record CreateHoldDTO(string Name, Guid TermId, Guid SubjectId);
    public record UpdateHoldDTO(Guid Id, string Name, Guid TermId, Guid SubjectId);
}
