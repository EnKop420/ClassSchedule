using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles all the CRUD business logic along with all the Session and Authentication functions.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a User with a specific role to a Institution.
        /// </summary>
        /// <param name="institutionId">The Institution to create the user for</param>
        /// <param name="dto">Contains all of the User values as raw values</param>
        public Task Add(Guid institutionId, CreateUserDTO dto);

        /// <summary>
        /// Creates an Admin User
        /// </summary>
        /// <param name="dto">Contains all the User values as raw values</param>
        public Task AddAdmin(CreateUserAdminDTO dto);

        /// <summary>
        /// Updates all the user information except the Username and Password
        /// </summary>
        /// <param name="userId">The specific user</param>
        /// <param name="institutionId">The specific Institution</param>
        /// <param name="dto">Contains all the UserInformation except Username and Password</param>
        /// <returns>The updated User</returns>
        public Task<UserDTO> UpdateUserInformation(Guid userId, Guid institutionId, UpdateUserInformationDTO dto);

        /// <summary>
        /// Changes the User's Username and Password
        /// </summary>
        /// <param name="userId">The specific user</param>
        /// <param name="institutionId">The specific Institution</param>
        /// <param name="dto">Contains the User Credentials to update to.</param>
        /// <returns>The updated Username</returns>
        public Task<string> ChangeUserCredentials(Guid userId, Guid institutionId, ChangeUserCredentialsDTO dto);

        /// <summary>
        /// Deletes a User
        /// </summary>
        /// <param name="id">The specific User</param>
        /// <param name="institutionId">The specific Institution</param>
        public Task Delete(Guid id, Guid institutionId);

        /// <summary>
        /// Deletes an Admin User
        /// </summary>
        /// <param name="id">The specific User</param>
        /// <param name="institutionId">The specific Institution</param>
        public Task DeleteAdmin(Guid id);

        /// <summary>
        /// Authenticates the User using the User Credentials.
        /// Creates a session to the Redis database and sets the cookie
        /// </summary>
        /// <param name="input">Contains the Username and Password and Institution</param>
        /// <param name="cookies">Used to set the cookie</param>
        /// <returns>Returns the gathered User Information.</returns>
        public Task<UserDTO> Login(LoginDTO input, IResponseCookies cookies);

        /// <summary>
        /// Deletes the Session Key from the redis database essentially logging the user out
        /// </summary>
        /// <param name="sessionKey">The session key to delete</param>
        public Task Logout(string sessionKey);

        /// <summary>
        /// Get the User information from Id
        /// </summary>
        /// <param name="targetId">The User to get the information from</param>
        /// <param name="callerId">The User who made the request</param>
        /// <param name="institutionId">The specific Institution</param>
        /// <param name="role">The role of caller to check if they are authorized</param>
        /// <returns>The User information</returns>
        public Task<UserDTO> GetUserInfo(Guid targetId, Guid callerId, Guid institutionId, UserRoles role);

        /// <summary>
        /// Gets all Users. Optionally can filter by role
        /// </summary>
        /// <param name="institutionId">The institution Id</param>
        /// <param name="currentUserRole">The caller's role to check if they are authorized</param>
        /// <param name="role">Optional if only a specific role is wanted</param>
        /// <returns>A list of all the Users</returns>
        public Task<List<UserDTO>> GetAllUsers(Guid institutionId, UserRoles currentUserRole, UserRoles? role = null);
    }
}
