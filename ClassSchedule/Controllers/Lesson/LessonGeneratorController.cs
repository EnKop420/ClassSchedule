using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System.Xml.Linq;

namespace ClassSchedule.Controllers.Lesson
{
    [Route("api/terms/{termId}/lessons")]
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
        public async Task<IActionResult> Generate(Guid termId)
        {
            try
            {
                int created = await _lessonGenerationService.GenerateForTermAsync(CurrentInstitutionId, termId);
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
    }
}
