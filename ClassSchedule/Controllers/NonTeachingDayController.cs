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
    [Route("api/NonTeachingDay")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class NonTeachingDayController : BaseController
    {
        private readonly INonTeachingDayService _nonTeachingDayService;
        public NonTeachingDayController(INonTeachingDayService nonTeachingDayService)
        {
            _nonTeachingDayService = nonTeachingDayService;
        }

        [HttpGet("get-all-nonteachingday")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _nonTeachingDayService.GetAllAsync(CurrentInstitutionId));
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

        [HttpGet("get-nonteachingday")]
        public async Task<IActionResult> GetById([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _nonTeachingDayService.GetByIdAsync(CurrentInstitutionId, id));
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

        [HttpPost("create-nonteachingday")]
        public async Task<IActionResult> Create([FromBody] CreateNonTeachingDayDTO dto)
        {
            try
            {
                return Ok(await _nonTeachingDayService.CreateAsync(CurrentInstitutionId, dto));
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

        [HttpPatch("update-nonteachingday")]
        public async Task<IActionResult> Update([FromBody] NonTeachingDayDTO dto)
        {
            try
            {
                return Ok(await _nonTeachingDayService.UpdateAsync(CurrentInstitutionId, dto));
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

        [HttpDelete("delete-nonteachingday")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _nonTeachingDayService.DeleteAsync(CurrentInstitutionId, id));
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
