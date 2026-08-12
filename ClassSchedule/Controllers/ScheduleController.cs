using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.Enums;

namespace ClassSchedule.Controllers
{
    [Route("api/Schedule")]
    [ApiController]
    [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
    public class ScheduleController : BaseController
    {
        [HttpGet("get")]
        public async Task<IActionResult> GetSchedule([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            try
            {
                // Placeholder for actual schedule retrieval logic
                var schedule = new { Message = "Schedule data would be here." };
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
