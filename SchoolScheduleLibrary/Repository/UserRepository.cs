using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SchoolDbContext _context;
        public UserRepository(SchoolDbContext context)
        {
            _context = context;    
        }

        public async Task<bool> DoesUsernameExist(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> Login(LoginDTO loginDTO)
        {
            Guid userId = Guid.Empty;
            string unauthenticatedMessage = "No match found for username and password!";

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDTO.Username && u.Password == loginDTO.Password);
            if (user == null) throw new HttpResponseException(ServiceReturnCode.Forbidden, unauthenticatedMessage);
            else return user;
        }
    }
}
