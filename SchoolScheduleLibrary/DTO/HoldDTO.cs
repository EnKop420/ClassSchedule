using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record HoldDTO(Guid Id, string Name, Guid SubjectId, Guid TermId, string SubjectName, string TermName, List<Guid> Teachers, List<Guid> Students);
    public record CreateHoldDTO(string Name, Guid TermId, Guid SubjectId, List<Guid> Teachers, List<Guid> Students);
}
