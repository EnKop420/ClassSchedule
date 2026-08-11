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
    public interface IUserService
    {
        public Task Add(CreateUserDTO dto);

        public Task Delete(Guid id);

        public Task<UserDTO> Login(LoginDTO input);

        public Task<string> CreateSession(SessionData sessionData, TimeSpan ttl);

        public Task<UserDTO> GetUserInfo(Guid id, Guid currentUserId, UserRoles role);

        public Task<List<UserDTO>> GetAllUsers(Guid institutionId);
    }
}
