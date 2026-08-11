using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/User")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] CreateUserDTO userDTO)
        {
            try
            {
                await _userService.Add(userDTO);
                return Ok();
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}\n Inner error message:\n{ex.InnerException}");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                int ttlDays = 3;

                UserDTO userDTO = await _userService.Login(loginDTO);
                SessionData data = new(userDTO.Id.ToString(), userDTO.Role, userDTO.InstitutionId.ToString());
                string sessionKey = await _userService.CreateSession(data, TimeSpan.FromDays(ttlDays));

                CookieOptions sessionCookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true, // Only sent over HTTPS. But for development this is disabled.
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(ttlDays)
                };

                //CookieOptions normalCookieOption = new CookieOptions
                //{
                //    HttpOnly = false,
                //    //Secure = true, // Only sent over HTTPS. But for development this is disabled.
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTime.UtcNow.AddDays(ttlDays)
                //};

                //UserCookieData userCookieData = new(userDTO.FirstName, userDTO.LastName, userDTO.DateOfBirth, user)

                Response.Cookies.Append("SchoolSession", sessionKey, sessionCookieOption);
                //Response.Cookies.Append("SchoolSchedule", , sessionCookieOption);

                return Ok(userDTO);
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

        [Authorize(UserRoles.Admin)]
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

        [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        [HttpGet("Get-User-Information")]
        public async Task<IActionResult> GetUserInformation(Guid id)
        {
            try
            {
                return Ok(await _userService.GetUserInfo(id, CurrentUserId, CurrentUserRole));
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

        // Development Function will not be included in the real production endpoint.
        [LocalhostOnly]
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] Guid institutionId)
        {
            try
            {
                return Ok(await _userService.GetAllUsers(institutionId));
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
