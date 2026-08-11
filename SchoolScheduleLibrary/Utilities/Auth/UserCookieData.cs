using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Auth
{
    public class UserCookieData
    {
        public string FullName { get; set; }
        public DateOnly DateOfBirth {  get; set; }
        public string InstitutionName { get; set; }
        public UserRoles Role {  get; set; }
        public string Username { get; set; }

        public UserCookieData(string firstName, string lastName, DateOnly dateofBirth, string institutionName, UserRoles role, string username)
        {
            FullName = $"{firstName} {lastName}";
            DateOfBirth = dateofBirth;
            InstitutionName = institutionName;
            Role = role;
            Username = username;
        }
    }
}
