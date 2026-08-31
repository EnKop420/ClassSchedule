using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/Hold/{holdId}")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class HoldMemberController : BaseController
    {
        private readonly IHoldMemberService _holdMemberService;
        public HoldMemberController(IHoldMemberService holdMemberService)
        {
            _holdMemberService = holdMemberService;
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetEnrolledStudents(Guid holdId)
        {
            try
            {
                return Ok(await _holdMemberService.GetStudentsAsync(holdId));
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

        [HttpGet("teachers")]
        public async Task<IActionResult> GetGroupedTeachers(Guid holdId)
        {
            try
            {
                return Ok(await _holdMemberService.GetTeachersAsync(holdId));
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
