using SchoolScheduleLibrary.DTO;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for StudentGroupMember
    /// </summary>
    public interface IStudentGroupMemberService
    {
        /// <summary>
        /// Gets a list of students from a StudentGroup
        /// </summary>
        /// <param name="StudentGroupId">The StudentGroup's students to get</param>
        /// <returns>A list of students</returns>
        public Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid StudentGroupId);
    }
}
