using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record HoldMemberDTO(string Name, Guid UserId, Guid HoldId);
}
