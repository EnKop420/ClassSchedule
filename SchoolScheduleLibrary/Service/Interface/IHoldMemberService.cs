using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IHoldMemberService
    {
        public Task<bool> EnrollStudentAsync(Guid institutionId, Guid holdId, List<Guid> studentIds);
        public Task<bool> UnenrollStudentAsync(Guid institutionId, Guid holdId, List<Guid> studentIds);
        public Task<bool> GroupTeacherAsync(Guid institutionId, Guid holdId, List<Guid> teacherIds);
        public Task<bool> UngroupTeacherAsync(Guid institutionId, Guid holdId, List<Guid> teacherIds);
        public Task<List<MinimalUserInformationDTO>> GetTeachersAsync(Guid holdId);
        public Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid holdId);
    }
}
