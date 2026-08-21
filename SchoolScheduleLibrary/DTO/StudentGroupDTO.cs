using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record StudentGroupDTO(Guid Id, string Name, List<User> UserList);
    public record CreateStudentGroupDTO(string Name, List<User> UserList);
    public record UpdateStudentGroupDTO(Guid Id, string Name);
}
