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
        Guid InstitutionId,
        string InstitutionName
    );

    public record CreateUserDTO(
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        [MinLength(4), MaxLength(16)] string Username,
        [MinLength(8), MaxLength(16)] string Password,
        [EmailAddress] string Email,
        UserRoles Role
    );

    public record CreateUserAdminDTO(
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        [MinLength(4), MaxLength(16)] string Username,
        [MinLength(8), MaxLength(16)] string Password,
        [EmailAddress] string Email,
        Guid InstitutionId
    );

    public record UpdateUserInformationDTO(
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        [EmailAddress] string Email);

    public record ChangeUserCredentialsDTO(
        string Username,
        string OldPassword,
        string NewPassword
    );

    public record MinimalUserInformationDTO(string Name, Guid UserId);
}
