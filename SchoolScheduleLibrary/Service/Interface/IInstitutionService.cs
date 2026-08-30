using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    /// <summary>
    /// Handles the CRUD business logic for Institution
    /// </summary>
    public interface IInstitutionService
    {
        /// <summary>
        /// Gets all of the institutions
        /// </summary>
        /// <returns>A list of institutions</returns>
        public Task<List<InstitutionDTO>> GetAllInstitutions();

        /// <summary>
        /// Gets a specific institution from an Id
        /// </summary>
        /// <param name="id">The institution's Id</param>
        /// <returns>The specific Institution</returns>
        public Task<InstitutionDTO> GetInstitutionById(Guid id);

        /// <summary>
        /// Creates an Institution
        /// </summary>
        /// <param name="dto">The data used to create an institution</param>
        public Task CreateInstitution(CreateInstitutionDTO dto);

        /// <summary>
        /// Updates an existing Institution with new data
        /// </summary>
        /// <param name="dto">The new values to update the institution with</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> UpdateInstitution(InstitutionDTO dto);

        /// <summary>
        /// Deletes an institution
        /// </summary>
        /// <param name="id">The specific institution's Id</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteInstitution(Guid id);
    }
}
