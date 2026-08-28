using SchoolScheduleLibrary.DTO;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for StudentGroupMember
    /// </summary>
    public interface IStudentGroupMemberService
    {
        /// <summary>
        /// Adds a list of students user id into a StudentGroup
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="StudentGroupId">The current StudentGroup to add the students to</param>
        /// <param name="studentIds">The list of the students to add</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> AddStudentListAsync(Guid institutionId, Guid StudentGroupId, List<Guid> studentIds);

        /// <summary>
        /// Removes a list of students from the StudentGroup
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="studentGroupId">The current StudentGroup to remove the students from</param>
        /// <param name="studentIds">The list of the students to remove</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> RemoveStudentListAsync(Guid institutionId, Guid studentGroupId, List<Guid> studentIds);

        /// <summary>
        /// Gets a list of students from a StudentGroup
        /// </summary>
        /// <param name="StudentGroupId">The StudentGroup's students to get</param>
        /// <returns>A list of students</returns>
        public Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid StudentGroupId);
    }
}
