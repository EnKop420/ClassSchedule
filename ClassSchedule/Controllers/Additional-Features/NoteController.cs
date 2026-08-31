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
    [Route("api/Note")]
    [ApiController]
    [Authorize(UserRoles.Teacher)]
    public class NoteController : BaseController
    {
        private readonly ILessonNoteService _lessonNoteService;

        public NoteController(ILessonNoteService lessonService)
        {
            _lessonNoteService = lessonService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddNoteToLesson([FromBody] CreateLessonNoteDTO dto)
        {
            try
            {
                return Ok(await _lessonNoteService.AddNoteToLesson(CurrentUserId, dto));
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

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateNoteFromLesson([FromBody] UpdateLessonNoteDTO dto)
        {
            try
            {
                return Ok(await _lessonNoteService.UpdateNoteFromLesson(CurrentUserId, dto));
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

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteNoteFromLesson([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _lessonNoteService.RemoveNoteFromLesson(id));
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
