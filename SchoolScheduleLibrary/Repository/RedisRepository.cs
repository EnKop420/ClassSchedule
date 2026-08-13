using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Response;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Repository
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _redisDb;

        public RedisRepository(IConnectionMultiplexer redisDb)
        {
            _connection = redisDb;
            _redisDb = _connection.GetDatabase();
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

        public async Task DeleteAllSessionsFromUserId(string userId)
        {
            // We know there is only 1 standalone server running redis. So only get 1
            EndPoint endpoint = _connection.GetEndPoints().Single(); // Endpoint is just the network address <IP>:<Port> e.g. 127.0.0.1:1234

            // IDatabase is used only for GET, SET, DEL actions while IServer is for server scoped commands like SCAN, KEYS, CONFIG etc.
            IServer server = _connection.GetServer(endpoint); // get a handle to THAT specific server

            var keysToDelete = new List<RedisKey>();

            foreach (var key in server.Keys(database: _redisDb.Database, pattern: "*", pageSize: 250))
            {
                RedisValue value = await _redisDb.StringGetAsync(key);
                if (!value.HasValue) continue;

                SessionData? session = JsonSerializer.Deserialize<SessionData>((string)value!);
                if (session != null 
                    && string.Equals(session.UserId, userId, StringComparison.OrdinalIgnoreCase)) keysToDelete.Add(key);
            }

            if (keysToDelete.Count > 0)
                await _redisDb.KeyDeleteAsync(keysToDelete.ToArray());
        }
    }
}
