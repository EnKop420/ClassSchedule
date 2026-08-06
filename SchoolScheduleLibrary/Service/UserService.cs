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
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _genericRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly IRedisRepository _redisRepository;

        public UserService(
            IGenericRepository<User> genericRepository,
            IUserRepository userRepository,
            IEncryptionHandler encryptionHandler,
            IRedisRepository redisRepository
            )
        {
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _encryptionHandler = encryptionHandler;
            _redisRepository = redisRepository;
        }

        public async Task Add(CreateUserDTO input)
        {
            if (!Enum.IsDefined(typeof(UserRoles), input.Role)) throw new BadRequestException("This role is not defined as a valid role!");
            else if (!await _genericRepository.DoesValueExist<Institution>(input.InstitutionId)) throw new NotFoundException($"No institution exists with Id \"{input.InstitutionId}\".");
            string lowerUsername = input.Username.ToLower();
            string hashedPassword = await _encryptionHandler.HashString(input.Password);
            string encryptedEmail = await _encryptionHandler.EncryptString(input.Email);

            bool doesUsernameExist = await _userRepository.DoesUsernameExist(lowerUsername);
            if (doesUsernameExist) throw new ConflictException("Username already exists!");

            User user = new User
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                DateOfBirth = input.DateOfBirth,
                Username = lowerUsername,
                Password = hashedPassword,
                Email = encryptedEmail,
                Role = input.Role,
                InstitutionId = input.InstitutionId
            };

            await _genericRepository.Create(user);
        }

        public async Task Delete(Guid id)
        {
            

            User? user = await _genericRepository.GetByGuid(id);
            if (user != null)
            {
                if (!await _genericRepository.DeleteById(id))
                {
                    throw new InternalErrorException("Something went wrong with deleting value! Id matches a user but unknown error");
                }
                else
                {
                    // TODO DELETE ALL SESSIONS KEY THAT IS ASSOCIATED WITH THE USER ID.
                }
            }
            else throw new NotFoundException($"No User with this Id \"{id}\" was found");
        }

        public async Task<UserDTO> Login(LoginDTO dto)
        {
            string hashedPassword = await _encryptionHandler.HashString(dto.Password);
            LoginDTO updatedDTO = dto with { Username = dto.Username.ToLower(), Password = hashedPassword };

            User user = await _userRepository.Login(updatedDTO);

            UserDTO userDTO = new (
                user.Id,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.Username,
                await _encryptionHandler.DecryptString(user.Email),
                user.CreatedAt,
                user.Role,
                user.InstitutionId
            );

            return userDTO;
        }

        public async Task<string> CreateSession(SessionData sessionData, TimeSpan ttl)
        {
            return await _redisRepository.AddSessionToDB(sessionData, ttl);
        }
    }
}
