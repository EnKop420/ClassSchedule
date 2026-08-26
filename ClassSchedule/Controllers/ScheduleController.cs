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
    [Route("api/Schedule")]
    [ApiController]
    [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetSchedule([FromQuery] Guid targetUserId, [FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            try
            {
                GetScheduleLessonDTO dto = new(targetUserId, from, to);
                return Ok(await _scheduleService.GetScheduleAsync(CurrentInstitutionId, CurrentUserId, CurrentUserRole, dto));
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
