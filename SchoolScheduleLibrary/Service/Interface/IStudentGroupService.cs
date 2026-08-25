using SchoolScheduleLibrary.DTO;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for StudentGroup
    /// </summary>
    public interface IStudentGroupService
    {
        /// <summary>
        /// Creates the StudentGroup while also adding the students
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">Contains the Name and list of students</param>
        /// <returns>Returns the student group.</returns>
        public Task<StudentGroupDTO> CreateAsync(Guid institutionId, CreateStudentGroupDTO dto);

        /// <summary>
        /// Updates the StudentGroup while also updating the students
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="dto">Contains the Id, Name and list of students</param>
        /// <returns>Returns the updated StudentGroup</returns>
        public Task<StudentGroupDTO> UpdateAsync(Guid institutionId, UpdateStudentGroupDTO dto);

        /// <summary>
        /// Deletes the StudentGroup
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The StudentGroup to delete's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteAsync(Guid institutionId, Guid id);

        /// <summary>
        /// Get's the specific StudentGroup from the Id
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <param name="id">The specific Id of the StudentGroup</param>
        /// <returns>Returns the StudentGroup</returns>
        public Task<StudentGroupDTO> GetByIdAsync(Guid institutionId, Guid id);

        /// <summary>
        /// Get's a list of all the StudentGroup's in the institution
        /// </summary>
        /// <param name="institutionId">The Institution to look in</param>
        /// <returns>A list of the StudentGroups</returns>
        public Task<List<StudentGroupDTO>> GetAllAsync(Guid institutionId);
    }
}
