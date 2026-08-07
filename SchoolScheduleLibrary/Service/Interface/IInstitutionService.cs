using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IInstitutionService
    {
        public Task<List<InstitutionDTO>> GetAllInstitutions();
        public Task<InstitutionDTO> GetInstitutionById(Guid id);
        public Task CreateInstitution(string name);
        public Task<InstitutionDTO> UpdateInstitution(InstitutionDTO dto);
        public Task<bool> DeleteInstitution(Guid id);
    }
}
