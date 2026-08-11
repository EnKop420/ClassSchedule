using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Response;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Repository
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redisDb;

        public RedisRepository(IConnectionMultiplexer redisDb)
        {
            _redisDb = redisDb.GetDatabase();
        }
        public async Task<bool> AddSessionToDB(string key, string sessionValue, TimeSpan ttl)
        {
            // Try to create the key only if it doesn't already exist
            return await _redisDb.StringSetAsync(
                key: key,
                value: sessionValue,
                expiry: ttl,
                when: When.NotExists
            );
        }

        public async Task<SessionData> GetSessionDataFromKey(string sessionKey)
        {
            RedisValue value = await _redisDb.StringGetAsync($"session:{sessionKey}");
            if ( value.IsNullOrEmpty) throw new NotFoundException("This session key does not exist!");
            else if (value.HasValue)
            {
                SessionData? session = JsonSerializer.Deserialize<SessionData>((string)value!);
                if (session != null) return session;
                else throw new NoContentException("Session key exists but could not deserialize value!");
            }
            else throw new NoContentException("Session key exists but no value exists!");

        }

        public async Task<bool> DeleteSessionFromDB(string sessionKey)
        {
            return await _redisDb.KeyDeleteAsync(sessionKey);
        }

        public async Task<bool> ValidateSession(string sessionKey)
        {
            return await _redisDb.KeyExistsAsync($"session:{sessionKey}");
        }
    }
}
