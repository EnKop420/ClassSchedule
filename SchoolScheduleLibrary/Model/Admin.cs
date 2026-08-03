using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    public class Admin : IUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public DateOnly Created { get; set; }
    }
}
