using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
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
            if (!Enum.IsDefined(typeof(UserRoles), input.Role)) throw new HttpResponseException(ServiceReturnCode.BadRequest, "This role is not defined as a valid role!");

            input.Username = input.Username.ToLower();
            bool doesUsernameExist = await _genericRepository.DoesUsernameExist<T>(input.Username);

            input.Password = await _encryptionHandler.HashString(input.Password.ToLower());
            input.Email = await _encryptionHandler.EncryptString(input.Email);

            if (doesUsernameExist) throw new HttpResponseException(ServiceReturnCode.Conflict, "Username already exists!");
            await _genericRepository.Create(input, true);
        }

        public async Task Delete(Guid id)
        {
            IUser? user = await _genericRepository.GetByGuid(id);
            if (user != null)
            {
                if (!await _genericRepository.DeleteById(id))
                {
                    throw new HttpResponseException(ServiceReturnCode.InternalError, "Something went wrong with deleting value! Id matches a user but unknown error");
                }
            }
            else throw new HttpResponseException(ServiceReturnCode.NotFound, $"No User with this Id \"{id}\" was found");
        }

        public async Task<IUser> Login(LoginDTO input)
        {
            input.Username = input.Username.ToLower();
            input.Password = await _encryptionHandler.HashString(input.Password.ToLower());
            IUser user = await _genericRepository.Login<T>(input);
            user.Email = await _encryptionHandler.DecryptString(user.Email);
            user.Password = "**********";
            return user;
        }

        public async Task<string> CreateSession(SessionData sessionData, TimeSpan ttl)
        {
            return await _redisRepository.AddSessionToDB(sessionData, ttl);
        }
    }
}
