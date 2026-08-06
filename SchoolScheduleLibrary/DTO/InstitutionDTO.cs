using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record CreateInstitutionDTO(string Name);
    public record InstitutionDTO(Guid Id, string Name);
}
