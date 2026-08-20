using ClassSchedule.Auth;
using ClassSchedule.Inheritance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using SchoolScheduleLibrary.Utilities.Response;

namespace ClassSchedule.Controllers
{
    [Route("api/holds/{holdId}")]
    [ApiController]
    [Authorize(UserRoles.Admin)]
    public class HoldMemberController : BaseController
    {
        private readonly IHoldMemberService _holdMemberService;
        public HoldMemberController(IHoldMemberService holdMemberService)
        {
            _holdMemberService = holdMemberService;
        }

        //[HttpPost("teacher/{teacherId}")]
        //public async Task<IActionResult> GroupTeacher(Guid holdId, Guid teacherId)
        //{
        //    try
        //    {
        //        return Ok(await _holdMemberService.GroupTeacherAsync(CurrentInstitutionId, holdId, teacherId));
        //    }
        //    catch (HttpResponseException hre)
        //    {
        //        return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        //[HttpDelete("teacher/{teacherId}")]
        //public async Task<IActionResult> UngroupTeacher(Guid holdId, Guid teacherId)
        //{
        //    try
        //    {
        //        return Ok(await _holdMemberService.UngroupTeacherAsync(CurrentInstitutionId, holdId, teacherId));
        //    }
        //    catch (HttpResponseException hre)
        //    {
        //        return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}



        //[HttpPost("student/{studentId}")]
        //public async Task<IActionResult> EnrollStudent(Guid holdId, Guid studentId)
        //{
        //    try
        //    {
        //        return Ok(await _holdMemberService.EnrollStudentAsync(CurrentInstitutionId, holdId, studentId));
        //    }
        //    catch (HttpResponseException hre)
        //    {
        //        return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        //[HttpDelete("student/{studentId}")]
        //public async Task<IActionResult> UnenrollStudent(Guid holdId, Guid studentId)
        //{
        //    try
        //    {
        //        return Ok(await _holdMemberService.UnenrollStudentAsync(CurrentInstitutionId, holdId, studentId));
        //    }
        //    catch (HttpResponseException hre)
        //    {
        //        return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpGet("student")]
        public async Task<IActionResult> GetEnrolledStudents(Guid holdId)
        {
            try
            {
                return Ok(await _holdMemberService.GetStudentsAsync(holdId));
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

        [HttpGet("teacher")]
        public async Task<IActionResult> GetGroupedTeachers(Guid holdId)
        {
            try
            {
                return Ok(await _holdMemberService.GetTeachersAsync(holdId));
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
