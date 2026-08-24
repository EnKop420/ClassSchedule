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
    [Route("api/Period")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class PeriodController : BaseController
    {
        private readonly IPeriodService _periodService;
        public PeriodController(IPeriodService periodService)
        {
            _periodService = periodService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _periodService.GetAllAsync(CurrentInstitutionId));
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

        [HttpGet("get")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _periodService.GetByIdAsync(CurrentInstitutionId, id));
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

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePeriodDTO dto)
        {
            try
            {
                return Ok(await _periodService.CreateAsync(CurrentInstitutionId, dto));
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
        public async Task<IActionResult> Update([FromBody] PeriodDTO dto)
        {
            try
            {
                return Ok(await _periodService.UpdateAsync(CurrentInstitutionId, dto));
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
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _periodService.DeleteAsync(CurrentInstitutionId, id));
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
