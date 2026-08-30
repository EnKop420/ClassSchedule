using Microsoft.AspNetCore.Http;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using System.Security.Cryptography;
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
            bool doesUsernameExist = await _userGenericRepository.DoesValueExist(u => u.Username == lowerUsername && u.InstitutionId == institutionId);

            if (doesUsernameExist) throw new ConflictException("Username already exists!");

            string hashedPassword = await _encryptionHandler.HashString(input.Password);
            string encryptedEmail = await _encryptionHandler.EncryptString(input.Email);

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

        public async Task<UserDTO> UpdateUserInformation(Guid userId, UpdateUserInformationDTO dto)
        {
            User user = await _userGenericRepository.Get(u => u.Id == userId, u => u.Institution)
                ?? throw new NotFoundException($"No User with this Id \"{userId}\" was found!");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.DateOfBirth = dto.DateOfBirth;
            user.Email = await _encryptionHandler.EncryptString(dto.Email);

            if (await _userGenericRepository.Update(user))
            {
                UserDTO userDTO = new(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.DateOfBirth,
                    user.Username,
                    await _encryptionHandler.DecryptString(user.Email),
                    user.CreatedAt,
                    user.Role,
                    user.InstitutionId,
                    user.Institution.Name);
                return userDTO;
            }
            else throw new InternalErrorException("Something went wrong with updating the user");
        }

        public async Task<string> ChangeUserCredentials(Guid userId, Guid institutionId, ChangeUserCredentialsDTO dto)
        {
            if (await _userGenericRepository.DoesValueExist(u => u.Username == dto.Username.ToLower() && u.InstitutionId == institutionId))
            {
                throw new ConflictException("Username already exists!");
            }

            string hashedOldPassword = await _encryptionHandler.HashString(dto.OldPassword);
            string hashedNewPassword = await _encryptionHandler.HashString(dto.NewPassword);

            User user = await _userGenericRepository.Get(u => u.Id == userId)
                ?? throw new NotFoundException($"No User with this Id \"{userId}\"");

            if (user.Password != hashedOldPassword) throw new UnauthorizedException("Password does not match with the current Password!");

            user.Username = dto.Username.ToLower();
            user.Password = hashedNewPassword;

            if (await _userGenericRepository.Update(user))
            {
                return dto.Username;
            }
            else throw new InternalErrorException("Something went wrong with updating the user's credentials");
        }

        public async Task Delete(Guid id)
        {
            User user = await _userGenericRepository.Get(u => u.Id == id)
                ?? throw new NotFoundException($"No User with this Id \"{id}\"");

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

        public async Task DeleteAdmin(Guid id)
        {
            User user = await _userGenericRepository.Get(u => u.Id == id)
                ?? throw new NotFoundException($"No User with this Id \"{id}\" was found!");

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

        private async Task<string> CreateSession(SessionData sessionData, TimeSpan ttl)
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

        public async Task<UserDTO> GetUserInfo(Guid targetId, Guid callerId, UserRoles role)
        {
            if (targetId != callerId && role == UserRoles.Student) throw new UnauthorizedException("Students can only get their own user information!");

            User user = await _userGenericRepository.Get(u => u.Id == targetId, u => u.Institution)
                ?? throw new NotFoundException($"No User with ID {targetId} exists");

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

        public async Task<List<UserDTO>> GetAllUsers(Guid institutionId, UserRoles currentUserRole, UserRoles? role = null)
        {
            List<User> users = [];

            if (role != null)
            {
                if (currentUserRole == UserRoles.Teacher && role == UserRoles.Admin) throw new UnauthorizedException("Teachers can not gather all Admin accounts!");

                users = await _userGenericRepository.GetAll(u => u.InstitutionId == institutionId && u.Role == role, u => u.Institution);
            }
            else
            {
                if (currentUserRole == UserRoles.Teacher)
                {
                    users = await _userGenericRepository.GetAll(u => u.InstitutionId == institutionId && u.Role != UserRoles.Admin, u => u.Institution);
                }
                else
                {
                    users = await _userGenericRepository.GetAll(u => u.InstitutionId == institutionId, u => u.Institution);
                }
            }

            List<UserDTO> userDTOs = new List<UserDTO>();
            foreach (User user in users)
            {
                userDTOs.Add(new UserDTO(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.DateOfBirth,
                    user.Username,
                    await _encryptionHandler.DecryptString(user.Email),
                    user.CreatedAt,
                    user.Role,
                    user.InstitutionId,
                    user.Institution.Name));
            }
            return userDTOs;
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
