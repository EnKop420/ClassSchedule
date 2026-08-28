using SchoolScheduleLibrary.Enums;

namespace SchoolScheduleLibrary.Model
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;

        public User(
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            string username,
            string password,
            string email,
            UserRoles role,
            Guid institutionId
        )
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            InstitutionId = institutionId;
        }
    }
}
