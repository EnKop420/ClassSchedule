using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IInstitutionService
    {
        public Task CreateInstitution(string name);
        public Task<List<InstitutionDTO>> GetAllInstitutions();
        public Task<InstitutionDTO> GetInstitutionById(Guid Id);
        public Task<bool> DeleteInstitution(Guid Id);
    }
}
