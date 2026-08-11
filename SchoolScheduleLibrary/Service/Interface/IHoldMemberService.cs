using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IHoldMemberService
    {
        public Task<bool> EnrollStudentAsync(Guid institutionId, Guid holdId, Guid studentId);
        public Task<bool> UnenrollStudentAsync(Guid institutionId, Guid holdId, Guid studentId);
        public Task<bool> GroupTeacherAsync(Guid institutionId, Guid holdId, Guid teacherId);
        public Task<bool> UngroupTeacherAsync(Guid institutionId, Guid holdId, Guid teacherId);
        public Task<List<HoldMemberDTO>> GetTeachersAsync(Guid holdId);
        public Task<List<HoldMemberDTO>> GetStudentsAsync(Guid holdId);
    }
}
