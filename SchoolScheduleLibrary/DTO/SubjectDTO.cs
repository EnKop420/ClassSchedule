using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record SubjectDTO(Guid Id, string Name);
    public record CreateSubjectDTO(string Name);
}
