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
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;
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

        [Authorize(UserRoles.Admin)]
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] CreateUserDTO dto)
        {
            try
            {
                if (dto.Role == UserRoles.Admin) throw new BadRequestException("You cannot make an Admin account!");

                await _userService.Add(CurrentInstitutionId, dto);
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

        [LocalhostOnly]
        [HttpPost("Add-Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] CreateUserAdminDTO dto)
        {
            try
            {
                await _userService.AddAdmin(dto);
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
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                UserDTO userDTO = await _userService.Login(dto, Response.Cookies);
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

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Request.Cookies.TryGetValue("SchoolSession", out string? sessionKey);
                if (sessionKey == null) throw new BadRequestException("No Session cookie!");

                await _userService.Logout(sessionKey);

                Response.Cookies.Delete("SchoolSession");
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

        [Authorize(UserRoles.Admin)]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            try
            {
                await _userService.Delete(id, CurrentInstitutionId);
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

        [LocalhostOnly]
        [HttpDelete("Delete-Admin")]
        public async Task<IActionResult> DeleteAdmin([FromBody] Guid id)
        {
            try
            {
                await _userService.DeleteAdmin(id);
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

        [Authorize]
        [HttpGet("Get-User-Information")]
        public async Task<IActionResult> GetUserInformation([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _userService.GetUserInfo(id, CurrentUserId, CurrentInstitutionId, CurrentUserRole));
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

        [Authorize(UserRoles.Admin, UserRoles.Teacher)]
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] Guid institutionId, [FromQuery] UserRoles? role = null)
        {
            try
            {
                return Ok(await _userService.GetAllUsers(institutionId, CurrentUserRole, role));
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

        [Authorize]
        [HttpPatch("Update-User-Information")]
        public async Task<IActionResult> UpdateUserInformation([FromBody] UpdateUserInformationDTO dto)
        {
            try
            {
                return Ok(await _userService.UpdateUserInformation(CurrentUserId, CurrentInstitutionId, dto));
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

        [Authorize]
        [HttpPatch("Change-User-Credentials")]
        public async Task<IActionResult> ChangeUserCredentials([FromBody] ChangeUserCredentialsDTO dto)
        {
            try
            {
                return Ok(await _userService.ChangeUserCredentials(CurrentUserId, CurrentInstitutionId, dto));
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
