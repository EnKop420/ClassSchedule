using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/Term")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class TermController : BaseController
    {
        private readonly ITermService _termService;
        public TermController(ITermService termService)
        {
            _termService = termService;
        }

        [HttpGet("get-all-terms")]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                return Ok(await _termService.GetAllAsync(CurrentInstitutionId));
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

        [HttpGet("get-term")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _termService.GetByIdAsync(CurrentInstitutionId, id));
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

        [HttpPost("create-term")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateTermDTO dto)
        {
            try
            {
                return Ok(await _termService.CreateAsync(CurrentInstitutionId, dto));
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

        [HttpPut("update-term")]
        public async Task<IActionResult> Update([FromBody] TermDTO dto)
        {
            try
            {
                return Ok(await _termService.UpdateAsync(CurrentInstitutionId, dto));
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

        [HttpDelete("delete-term")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _termService.DeleteAsync(CurrentInstitutionId, id));
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
