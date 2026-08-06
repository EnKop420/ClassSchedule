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

                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true, // Only sent over HTTPS. But for development this is disabled.
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(ttlDays)
                };

                Response.Cookies.Append("SchoolSession", sessionKey, cookieOptions);

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
    }
}
