using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Authentication;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IUserService<Admin> _userService;
        public StudentController(IUserService<Admin> adminService)
        {
            _userService = adminService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                int ttlDays = 7;
                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true, // Only sent over HTTPS. But for development this is disabled.
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(ttlDays)
                };

                Student student = (Student)await _userService.Login(loginDTO);
                SessionData data = new(student.Id.ToString(), Roles.STUDENT);
                string sessionKey = await _userService.CreateSession(data, TimeSpan.FromDays(ttlDays));

                Response.Cookies.Append("SchoolSession", sessionKey, cookieOptions);

                return Ok(student);
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
