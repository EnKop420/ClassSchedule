using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Utilities.Auth
{
    public class SessionData
    {
        public string UserId { get; set; }
        public string Role {  get; set; }

        public SessionData(string userId, string role)
        {
            UserId = userId;
            Role = role;
        }
    }
}
