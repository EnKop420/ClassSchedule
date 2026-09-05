using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System.Xml.Linq;

namespace ClassSchedule.Controllers.Lesson
{
    [Route("api/Generator")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class LessonGeneratorController : BaseController
    {
        private readonly ILessonGenerationService _lessonGenerationService;
        public LessonGeneratorController(ILessonGenerationService lessonGenerationService)
        {
            _lessonGenerationService = lessonGenerationService;
        }

        [HttpPost("generate-lessons")]
        public async Task<IActionResult> Generate(GenerateLessonDTO dto)
        {
            try
            {
                int created = await _lessonGenerationService.GenerateForTermAsync(CurrentInstitutionId, dto);
                return Ok(new { created });
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

        [HttpDelete("delete-lessons")]
        public async Task<IActionResult> DeleteLessons([FromBody] DeleteLessonDTO dto)
        {
            try
            {
                int deleted = await _lessonGenerationService.DeleteGeneratedLessons(dto);
                return Ok(new { deleted });
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
