using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public class AdminDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
