using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IUserService<T> where T : class, IUser // T has to be a class and has to implement IUser
    {
        public Task Add(T user);

        public Task Delete(Guid id);

        public Task<IUser> Login(LoginDTO input);

        public Task<string> CreateSession(SessionData sessionData, TimeSpan ttl);
    }
}
