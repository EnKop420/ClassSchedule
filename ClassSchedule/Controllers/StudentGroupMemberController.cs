using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/studentgroups/{studentGroupId}")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class StudentGroupMemberController : BaseController
    {
        private readonly IStudentGroupMemberService _studentGroupMemberService;
        public StudentGroupMemberController(IStudentGroupMemberService studentGroupMemberService)
        {
            _studentGroupMemberService = studentGroupMemberService;
        }

        [HttpPost("add-students")]
        public async Task<IActionResult> AddStudent(Guid studentGroupId, List<Guid> studentIds)
        {
            try
            {
                return Ok(await _studentGroupMemberService.AddStudentListAsync(CurrentInstitutionId, studentGroupId, studentIds));
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

        [HttpDelete("student/{studentId}")]
        public async Task<IActionResult> RemoveStudent(Guid studentGroupId, Guid studentId)
        {
            try
            {
                return Ok(await _studentGroupMemberService.RemoveStudentAsync(CurrentInstitutionId, studentGroupId, studentId));
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

        [HttpGet("students")]
        public async Task<IActionResult> GetEnrolledStudents(Guid studentGroupId)
        {
            try
            {
                return Ok(await _studentGroupMemberService.GetStudentsAsync(studentGroupId));
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
