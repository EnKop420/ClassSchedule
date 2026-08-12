using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class InstitutionService : IInstitutionService
    {
        private readonly IGenericRepository<Institution> _genericRepository;
        public InstitutionService(IGenericRepository<Institution> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task CreateInstitution(CreateInstitutionDTO dto)
        {
            Institution institution = new(dto.Name);

            await _genericRepository.Add(institution);
        }

        public async Task<bool> DeleteInstitution(Guid id)
        {
            if (!await _genericRepository.DoesValueExist(i => i.Id == id)) throw new NotFoundException($"Institution with Id \"{id}\" was not found!");
            return await _genericRepository.Delete(i => i.Id == id);
        }

        public async Task<List<InstitutionDTO>> GetAllInstitutions()
        {
            List<Institution> institutions = await _genericRepository.GetAll();

            return institutions
                .Select(i => new InstitutionDTO(i.Id, i.Name))
                .ToList();
        }

        public async Task<InstitutionDTO> GetInstitutionById(Guid id)
        {
            Institution? institution = await _genericRepository.Get(i => i.Id == id);
            if (institution == null) throw new NotFoundException($"Institution with Id \"{id}\" was not found!");

            return new InstitutionDTO(institution.Id, institution.Name);
        }

        public async Task<InstitutionDTO> UpdateInstitution(InstitutionDTO dto)
        {
            Institution institution = await _genericRepository.Get(i => i.Id == dto.Id) ?? throw new NotFoundException($"Institution with Id \"{dto.Id}\" was not found!");

            institution.Name = dto.Name;

            Institution updatedInstitution = await _genericRepository.Update(institution);

            return new InstitutionDTO(updatedInstitution.Id, updatedInstitution.Name);
        }
    }
}
