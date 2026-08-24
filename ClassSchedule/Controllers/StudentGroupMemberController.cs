using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/StudentGroup/{studentGroupId}")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class StudentGroupMemberController : BaseController
    {
        private readonly IStudentGroupMemberService _studentGroupMemberService;
        public StudentGroupMemberController(IStudentGroupMemberService studentGroupMemberService)
        {
            _studentGroupMemberService = studentGroupMemberService;
        }

        [HttpGet("get-students")]
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
