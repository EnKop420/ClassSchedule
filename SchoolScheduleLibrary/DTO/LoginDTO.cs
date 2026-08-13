using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record LoginDTO(string Username, string Password, Guid InstitutionId);
}
