using Microsoft.AspNetCore.Http;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IGenericRepository<Institution> _institutionGenericRepository;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly IRedisRepository _redisRepository;

        public UserService(
            IGenericRepository<User> userGenericRepository,
            IGenericRepository<Institution> institutionGenericRepository,
            IEncryptionHandler encryptionHandler,
            IRedisRepository redisRepository
            )
        {
            _userGenericRepository = userGenericRepository;
            _institutionGenericRepository = institutionGenericRepository;
            _encryptionHandler = encryptionHandler;
            _redisRepository = redisRepository;
        }

        public async Task Add(Guid institutionId, CreateUserDTO input)
        {
            if (!Enum.IsDefined(typeof(UserRoles), input.Role)) throw new BadRequestException("This role is not defined as a valid role!");

            if (!await _institutionGenericRepository.DoesValueExist(u => u.Id == institutionId)) throw new NotFoundException($"No institution exists with Id \"{institutionId}\".");

            string lowerUsername = input.Username.ToLower();
            bool doesUsernameExist = await _userGenericRepository.DoesValueExist(u => u.Username == lowerUsername);

            string hashedPassword = await _encryptionHandler.HashString(input.Password);
            string encryptedEmail = await _encryptionHandler.EncryptString(input.Email);

            if (doesUsernameExist) throw new ConflictException("Username already exists!");

            User user = new(
                input.FirstName,
                input.LastName,
                input.DateOfBirth,
                lowerUsername,
                hashedPassword,
                encryptedEmail,
                input.Role,
                institutionId
            );

            await _userGenericRepository.Add(user);
        }

        public async Task AddAdmin(CreateUserAdminDTO dto)
        {
            CreateUserDTO userDTO = new(
                dto.FirstName,
                dto.LastName,
                dto.DateOfBirth,
                dto.Username,
                dto.Password,
                dto.Email,
                UserRoles.Admin);

            await Add(dto.InstitutionId, userDTO);
        }

        public async Task Delete(Guid id)
        {
            User? user = await _userGenericRepository.Get(u => u.Id == id);
            if (user != null)
            {
                if (user.Role == UserRoles.Admin) throw new UnauthorizedException("You are not authorized to delete an Admin account!");

                if (!await _userGenericRepository.Delete(u => u.Id == id))
                {
                    throw new InternalErrorException("Something went wrong with deleting value! Id matches a user but unknown error");
                }
                else
                {
                    await _redisRepository.DeleteAllSessionsFromUserId(user.Id.ToString());
                }
            }
            else throw new NotFoundException($"No User with this Id \"{id}\" was found");
        }

        public async Task DeleteAdmin(Guid id)
        {
            User? user = await _userGenericRepository.Get(u => u.Id == id);
            if (user != null)
            {
                if (user.Role != UserRoles.Admin) throw new BadRequestException("You can only delete an admin account using this Endpoint!");

                if (!await _userGenericRepository.Delete(u => u.Id == id))
                {
                    throw new InternalErrorException("Something went wrong with deleting value! Id matches a user but unknown error");
                }
                else
                {
                    await _redisRepository.DeleteAllSessionsFromUserId(user.Id.ToString());
                }
            }
            else throw new NotFoundException($"No User with this Id \"{id}\" was found");
        }

        public async Task<UserDTO> Login(LoginDTO dto, IResponseCookies cookies)
        {
            int ttlDays = 3;

            string hashedPassword = await _encryptionHandler.HashString(dto.Password);
            string lowerUsername = dto.Username.ToLower();

            User user = await _userGenericRepository.Get(u => u.Username == lowerUsername && u.Password == hashedPassword && u.InstitutionId == dto.InstitutionId, u => u.Institution)
                ?? throw new UnauthorizedException("No match found for username and password in this institution!");

            UserDTO userDTO = new (
                user.Id,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.Username,
                await _encryptionHandler.DecryptString(user.Email),
                user.CreatedAt,
                user.Role,
                user.InstitutionId,
                user.Institution.Name
            );

            SessionData data = new(userDTO.Id.ToString(), userDTO.Role, userDTO.InstitutionId.ToString());
            string sessionKey = await CreateSession(data, TimeSpan.FromDays(ttlDays));

            CookieOptions sessionCookieOption = new CookieOptions
            {
                HttpOnly = true,
                //Secure = true, // Only sent over HTTPS. But for development this is disabled.
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(ttlDays)
            };

            cookies.Append("SchoolSession", sessionKey, sessionCookieOption);

            return userDTO;
        }

        public async Task<string> CreateSession(SessionData sessionData, TimeSpan ttl)
        {
            bool duplicateKey = true;
            string sessionId = string.Empty;
            do
            {
                sessionId = SessionIdGenerator();
                duplicateKey = await _redisRepository.ValidateSession(sessionId);
            }
            while (duplicateKey);

            string key = $"session:{sessionId}";
            string sessionValue = JsonSerializer.Serialize(sessionData);
            if (await _redisRepository.AddSessionToDB(key, sessionValue, ttl)) return sessionId;
            else throw new InternalErrorException("Something went wrong trying to create the session!");
        }

        public async Task<UserDTO> GetUserInfo(Guid id, Guid currentUserId, UserRoles role)
        {
            if (id != currentUserId && role == UserRoles.Student) throw new UnauthorizedException("Students can only get their own user information!");

            User user = await _userGenericRepository.Get(u => u.Id == id, u => u.Institution)
                ?? throw new NotFoundException($"No User with ID {id} exists");

            string decryptedEmail = await _encryptionHandler.DecryptString(user.Email);
            return new UserDTO(
                user.Id,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.Username,
                decryptedEmail,
                user.CreatedAt,
                user.Role,
                user.InstitutionId,
                user.Institution.Name);
        }

        public async Task<List<UserDTO>> GetAllUsers(Guid institutionId)
        {
            return (await _userGenericRepository.GetAll(u => u.InstitutionId == institutionId, u => u.Institution))
                .Select(u => new UserDTO(u.Id, u.FirstName, u.LastName, u.DateOfBirth, u.Username, u.Email, u.CreatedAt, u.Role, u.InstitutionId, u.Institution.Name)).ToList();
        }

        public async Task Logout(string key)
        {
            bool success = await _redisRepository.DeleteSessionFromDB($"session:{key}");
            if (success == false) throw new InternalErrorException("Something went wrong trying to delete the sessionKey");
        }

        // Generates a random string as a session id/key
        private static string SessionIdGenerator()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .TrimEnd('=');
        }
    }
}
