using SchoolScheduleLibrary.Utilities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    public interface IRedisRepository
    {
        public Task<bool> AddSessionToDB(string key, string sessionValue, TimeSpan ttl);

        public Task<SessionData> GetSessionDataFromKey(string key);

        public Task<bool> DeleteSessionFromDB(string sessionKey);

        public Task DeleteAllSessionsFromUserId(string inputValue);

        public Task<bool> ValidateSession(string sessionKey);
    }
}
