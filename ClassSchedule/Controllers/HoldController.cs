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
    [Route("api/Hold")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class HoldController : BaseController
    {
        private readonly IHoldService _holdService;
        public HoldController(IHoldService holdService)
        {
            _holdService = holdService;
        }

        [HttpGet("get-all-holds")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _holdService.GetAllAsync(CurrentInstitutionId));
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

        [HttpGet("get-hold")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _holdService.GetByIdAsync(CurrentInstitutionId, id));
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

        [HttpPost("create-hold")]
        public async Task<IActionResult> Create(CreateHoldDTO dto)
        {
            try
            {
                return Ok(await _holdService.CreateAsync(CurrentInstitutionId, dto));
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

        [HttpPut("update-hold")]
        public async Task<IActionResult> Update([FromBody] HoldDTO dto)
        {
            try
            {
                return Ok(await _holdService.UpdateAsync(CurrentInstitutionId, dto));
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

        [HttpDelete("delete-hold")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _holdService.DeleteAsync(CurrentInstitutionId, id));
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
