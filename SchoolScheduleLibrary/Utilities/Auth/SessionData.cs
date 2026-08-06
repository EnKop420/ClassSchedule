using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Auth
{
    public class SessionData
    {
        public string UserId { get; set; }
        public UserRoles Role {  get; set; }
        public string InstitutionId { get; set; }

        public SessionData(string userId, UserRoles role, string institutionId)
        {
            UserId = userId;
            Role = role;
            InstitutionId = institutionId;
        }
    }
}
