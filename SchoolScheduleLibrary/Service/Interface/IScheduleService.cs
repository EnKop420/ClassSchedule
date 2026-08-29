using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the GET business logic for the entire Schedule
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// Gets a list of all the lessons for a specific user.
        /// </summary>
        /// <param name="institutionId">The specific Institution</param>
        /// <param name="callerId">The caller's Id. Ensures students cannot fetch other students or teachers lessons data</param>
        /// <param name="callerRole">The Caller's role used to authorize the user</param>
        /// <param name="dto">Contains the target id and from and to dates</param>
        /// <returns>A List of all the Lessons formatted to be shown as a schedule</returns>
        public Task<List<ScheduleLessonDTO>> GetScheduleAsync(Guid institutionId, Guid callerId, UserRoles callerRole, GetScheduleLessonDTO dto);
    }
}
