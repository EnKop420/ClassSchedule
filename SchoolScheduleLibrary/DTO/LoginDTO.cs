using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record LoginDTO(
        [MinLength(4), MaxLength(16)] string Username,
        [MinLength(8), MaxLength(16)] string Password,
        Guid InstitutionId);
}
