using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers.Lesson
{
    [Route("api/Lesson")]
    [ApiController]
    public class LessonController : BaseController
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetLesson([FromQuery] Guid lessonId)
        {
            try
            {
                return Ok(await _lessonService.GetLesson(lessonId));
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
        [HttpGet("get-students")]
        public async Task<IActionResult> GetStudentsFromSchedule([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _lessonService.GetStudentsFromSchedule(id));
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

        [Authorize(UserRoles.Teacher)]
        [HttpPatch("change-status")]
        public async Task<IActionResult> ChangeLessonStatus([FromQuery] Guid lessonId, [FromBody] LessonStatus status)
        {
            try
            {
                return Ok(await _lessonService.ChangeLessonStatus(lessonId, status));
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
