using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// The service functions for the Many-To-Many table "Enrollment" and "GroupTeacher" for the HoldService.
    /// </summary>
    public interface IHoldMemberService
    {
        /// <summary>
        /// Get a list of all the teachers to the hold
        /// </summary>
        /// <param name="holdId">The hold it needs to get the teachers from</param>
        /// <returns>A list of all the teachers</returns>
        public Task<List<MinimalUserInformationDTO>> GetTeachersAsync(Guid holdId);

        /// <summary>
        /// Get a list of all the students to the hold
        /// </summary>
        /// <param name="holdId">The hold it needs to get the students from</param>
        /// <returns>A list of all the students</returns>
        public Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid holdId);
    }
}
