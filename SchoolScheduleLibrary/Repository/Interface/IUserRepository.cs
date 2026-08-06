using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<bool> DoesUsernameExist(string username);
        public Task<User> Login(LoginDTO loginDTO);
    }
}
