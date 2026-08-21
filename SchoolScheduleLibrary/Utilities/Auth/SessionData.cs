using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Auth
{
    /// <summary>
    /// Used by the Redis Repository and Login endpoint for making the Session Data to store in the Redis database.
    /// Contains all the information that is needed throughout the project
    /// </summary>
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
