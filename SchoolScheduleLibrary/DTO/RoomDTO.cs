using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record RoomDTO(Guid Id, string Name, int? Capacity);
    public record CreateRoomDTO(string Name, int? Capacity);
}
