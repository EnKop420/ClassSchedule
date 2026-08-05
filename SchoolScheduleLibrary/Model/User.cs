using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    public class User : IUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; }
    }
}
