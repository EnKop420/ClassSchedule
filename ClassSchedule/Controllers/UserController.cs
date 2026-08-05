using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Response;
using AuthorizeAttribute = ClassSchedule.Auth.AuthorizeAttribute;

namespace ClassSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService<User> _userService;
        public UserController(IUserService<User> userService)
        {
            _userService = userService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UserDTO userDTO)
        {
            try
            {
                User user = new User
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    DateOfBirth = userDTO.DateOfBirth,
                    Username = userDTO.Username,
                    Password = userDTO.Password,
                    Email = userDTO.Email,
                    Role = userDTO.Role,
                    CreatedAt = DateTime.UtcNow
                };
                await _userService.Add(user);
                return Ok();
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                int ttlDays = 3;
                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true, // Only sent over HTTPS. But for development this is disabled.
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(ttlDays)
                };

                User user = (User)await _userService.Login(loginDTO);
                SessionData data = new(user.Id.ToString(), user.Role);
                string sessionKey = await _userService.CreateSession(data, TimeSpan.FromDays(ttlDays));

                Response.Cookies.Append("SchoolSession", sessionKey, cookieOptions);

                return Ok(user);
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Role = UserRoles.Admin)]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok();
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
