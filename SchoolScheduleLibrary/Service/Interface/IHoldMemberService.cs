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
        /// Add a list of students to a hold and "Enrollment" table
        /// </summary>
        /// <param name="institutionId">institution id used to ensure its the same institution</param>
        /// <param name="holdId">The hold it needs to add the students to.</param>
        /// <param name="studentIds">The list of students user ids</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> EnrollStudentAsync(Guid institutionId, Guid holdId, List<Guid> studentIds);

        /// <summary>
        /// Remove a list of students from the hold and "Enrollment" table
        /// </summary>
        /// <param name="institutionId">institution id used to ensure its the same institution</param>
        /// <param name="holdId">The hold it needs to add the students to.</param>
        /// <param name="studentIds">The list of students user ids</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> UnenrollStudentAsync(Guid institutionId, Guid holdId, List<Guid> studentIds);

        /// <summary>
        /// Add a list of teachers to a hold and "GroupTeacher" table
        /// </summary>
        /// <param name="institutionId">institution id used to ensure its the same institution</param>
        /// <param name="holdId">The hold it needs to add the teacher to</param>
        /// <param name="teacherIds">The list of teachers user ids</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> GroupTeacherAsync(Guid institutionId, Guid holdId, List<Guid> teacherIds);

        /// <summary>
        /// Removes a list of teachers to a hold and "GroupTeacher" table
        /// </summary>
        /// <param name="institutionId">institution id used to ensure its the same institution</param>
        /// <param name="holdId">The hold it needs to add the teacher to.</param>
        /// <param name="teacherIds">The list of teachers user ids</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> UngroupTeacherAsync(Guid institutionId, Guid holdId, List<Guid> teacherIds);

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
