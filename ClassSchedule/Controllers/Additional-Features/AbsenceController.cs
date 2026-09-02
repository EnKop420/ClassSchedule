using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers.Lesson
{
    [Route("api/Absence")]
    [ApiController]
    [Authorize(UserRoles.Teacher)]
    public class AbsenceController : BaseController
    {
        private readonly IAbsenceService _absenceService;

        public AbsenceController(IAbsenceService absenceService)
        {
            _absenceService = absenceService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAbsencesFromLesson([FromQuery] Guid lessonId)
        {
            try
            {
                return Ok(await _absenceService.GetAllAbsencesFromLesson(lessonId));
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

        [HttpPut("set-absence")]
        public async Task<IActionResult> SetLessonAbsence([FromQuery] Guid lessonId, [FromBody] List<SetAbsenceDTO> dtos)
        {
            try
            {
                return Ok(await _absenceService.SetAbsence(lessonId, dtos, CurrentUserId));
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
