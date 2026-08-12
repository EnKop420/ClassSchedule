using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Auth;
using SchoolScheduleLibrary.Utilities.Response;
using System.Xml.Linq;

namespace ClassSchedule.Controllers
{
    [Route("api/Institution")]
    [ApiController]

    /**
     * Institution Controller made to perform CRUD action on the Institution data.
     * This data can only be created, modified or deleted from the Localhost port to limit its usage to only us "Product Owners"
     * The "Get" is available to all roles (Student, Teacher, Admin). However the "Get All" is still locked and only usable by "Product Owners"
     */
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionService _institutionService;
        public InstitutionController(IInstitutionService institutionService)
        {
            _institutionService = institutionService;
        }

        [HttpPost("create-institution")]
        [LocalhostOnly]
        public async Task<IActionResult> CreateInstitution([FromBody] CreateInstitutionDTO dto)
        {
            try
            {
                await _institutionService.CreateInstitution(dto);
                return Ok("Institution has been created successfully");
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

        [HttpPut("update-institution")]
        [LocalhostOnly]
        public async Task<IActionResult> UpdateInstitution([FromBody] InstitutionDTO dto)
        {
            try
            {
                return Ok(await _institutionService.UpdateInstitution(dto));
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
        public async Task<IActionResult> GetInstitution([FromQuery] Guid id)
        {
            try
            {
                return Ok(await _institutionService.GetInstitutionById(id));
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
