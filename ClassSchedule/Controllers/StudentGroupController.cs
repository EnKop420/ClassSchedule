using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/StudentGroup")]
    [ApiController]
    public class StudentGroupController : BaseController
    {
        private readonly IStudentGroupService _studentGroupService;
        public StudentGroupController(IStudentGroupService studentGroupService)
        {
            _studentGroupService = studentGroupService;
        }

        [Authorize(UserRoles.Admin)]
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] CreateStudentGroupDTO dto)
        {
            try
            {
                await _studentGroupService.CreateAsync(CurrentInstitutionId, dto);
                return Ok();
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}\n Inner error message:\n{ex.InnerException}");
            }
        }

        [Authorize(UserRoles.Admin)]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] Guid id)
        {
            try
            {
                await _studentGroupService.DeleteAsync(CurrentInstitutionId, id);
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

        [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        [HttpGet("Get-Student-Group-Information")]
        public async Task<IActionResult> Get([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _studentGroupService.GetByIdAsync(CurrentInstitutionId, id));
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

        [Authorize(UserRoles.Admin, UserRoles.Teacher, UserRoles.Student)]
        [HttpGet("Get-All-Student-Groups")]
        public async Task<IActionResult> GetAll([FromQuery] Guid institutionId)
        {
            try
            {
                return Ok(await _studentGroupService.GetAllAsync(institutionId));
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

        [Authorize(UserRoles.Admin)]
        [HttpPatch("Update-Student-Group-Information")]
        public async Task<IActionResult> UpdateStudentGroupInformation(UpdateStudentGroupDTO dto)
        {
            try
            {
                return Ok(await _studentGroupService.UpdateAsync(CurrentInstitutionId, dto));
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
