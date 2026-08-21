using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Enums
{
    /// <summary>
    /// The different roles a user can have made into an integer for the database.
    /// </summary>
    public enum UserRoles
    {
        Admin = 0,
        Teacher = 1,
        Student = 2
    }
}
