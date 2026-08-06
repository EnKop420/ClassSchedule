using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record UserDTO(
        Guid Id,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string Username,
        [EmailAddress] string Email,
        DateTime CreatedAt,
        UserRoles Role,
        Guid InstitutionId
    );

    public record CreateUserDTO(
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string Username,
        string Password,
        [EmailAddress] string Email,
        UserRoles Role,
        Guid InstitutionId
    );
}
