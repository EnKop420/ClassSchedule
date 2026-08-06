using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Response;
using System.Xml.Linq;

namespace ClassSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstitutionController : BaseController
    {
        private readonly IInstitutionService _institutionService;
        public InstitutionController(IInstitutionService institutionService)
        {
            _institutionService = institutionService;
        }

        [HttpPost("create-institution")]
        [LocalhostOnly]
        public async Task<IActionResult> CreateInstitution([FromBody] string name)
        {
            try
            {
                await _institutionService.CreateInstitution(name);
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

        [HttpGet("get-all-institution")]
        [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        public async Task<IActionResult> GetAllInstitutions()
        {
            try
            {
                return Ok(await _institutionService.GetAllInstitutions());
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

        [HttpGet("get-institution")]
        [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        public async Task<IActionResult> GetInstitution([FromQuery] Guid Id)
        {
            try
            {
                return Ok(await _institutionService.GetInstitutionById(Id));
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

        [HttpDelete("delete-institution")]
        [LocalhostOnly]
        public async Task<IActionResult> DeleteInstitution([FromBody] Guid Id)
        {
            try
            {
                return Ok(await _institutionService.DeleteInstitution(Id));
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
