using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Authentication;
using SchoolScheduleLibrary.Utilities.Response;
using AuthorizeAttribute = ClassSchedule.Auth.AuthorizeAttribute;

namespace ClassSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService<Admin> _userService;
        public AdminController(IUserService<Admin> adminService)
        {
            _userService = adminService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAdmin([FromBody] AdminDTO adminDTO)
        {
            try
            {
                Admin admin = new Admin
                {
                    Username = adminDTO.Username,
                    Password = adminDTO.Password,
                    Email = adminDTO.Email
                };

                await _userService.Add(admin);
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

                Admin admin = (Admin)await _userService.Login(loginDTO);
                SessionData data = new(admin.Id.ToString(), Roles.ADMIN);
                string sessionKey = await _userService.CreateSession(data, TimeSpan.FromDays(ttlDays));

                Response.Cookies.Append("SchoolSession", sessionKey, cookieOptions);

                return Ok(admin);
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

        [Authorize(Role = Roles.ADMIN)]
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
