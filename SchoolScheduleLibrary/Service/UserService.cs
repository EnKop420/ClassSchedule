using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Authentication;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service
{
    public class UserService<T> : IUserService<T> where T : class, IBaseEntity, IUser
    {
        private readonly IGenericRepository<T> _genericRepository;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly IRedisRepository _redisRepository;

        public UserService(
            IGenericRepository<T> genericRepository,
            IEncryptionHandler encryptionHandler,
            IRedisRepository redisRepository
            )
        {
            _genericRepository = genericRepository;
            _encryptionHandler = encryptionHandler;
            _redisRepository = redisRepository;
        }

        public async Task Add(T input)
        {
            input.Username = input.Username.ToLower();
            bool doesUsernameExist = await _genericRepository.DoesUsernameExist<T>(input.Username);

            input.Password = await _encryptionHandler.HashString(input.Password.ToLower());
            input.Email = await _encryptionHandler.EncryptString(input.Email);
            input.Created = DateOnly.FromDateTime(DateTime.Now);

            if (doesUsernameExist) throw new HttpResponseException(ServiceReturnCode.Conflict, "Username already exists!");
            await _genericRepository.Create(input, true);
        }

        public async Task Delete(Guid id)
        {
            await _genericRepository.DeleteById(id);
        }

        public async Task<IUser> Login(LoginDTO input)
        {
            input.Username = input.Username.ToLower();
            input.Password = await _encryptionHandler.HashString(input.Password.ToLower());
            return await _genericRepository.Login<T>(input);
        }

        public async Task<string> CreateSession(SessionData sessionData, TimeSpan ttl)
        {
            return await _redisRepository.AddSessionToDB(sessionData, ttl);
        }
    }
}
