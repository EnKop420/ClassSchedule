using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class SubjectsController : BaseController
    {
        private readonly ISubjectService _subjectService;
        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("get-all-subjects")]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                return Ok(await _subjectService.GetAllAsync(CurrentInstitutionId));
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

        [HttpPost("create-subject")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDTO dto)
        {
            try
            {
                return Ok(await _subjectService.CreateAsync(CurrentInstitutionId, dto));
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
