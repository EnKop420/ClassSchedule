using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
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

        public async Task CreateInstitution(string name)
        {
            Institution institution = new Institution
            {
                Name = name
            };

            await _genericRepository.Create(institution);
        }

        public async Task<bool> DeleteInstitution(Guid Id)
        {
            if (!await _genericRepository.DoesValueExist<Institution>(Id)) throw new NotFoundException($"Institution with Id \"{Id}\" was not found!");
            return await _genericRepository.DeleteById(Id);
        }

        public async Task<List<InstitutionDTO>> GetAllInstitutions()
        {
            List<Institution> institutions = await _genericRepository.GetAll();

            return institutions
                .Select(i => new InstitutionDTO(i.Id, i.Name))
                .ToList();
        }

        public async Task<InstitutionDTO> GetInstitutionById(Guid Id)
        {
            Institution? institution = await _genericRepository.GetByGuid(Id);
            if (institution == null) throw new NotFoundException($"Institution with Id \"{Id}\" was not found!");

            return new InstitutionDTO(institution.Id, institution.Name);
        }
    }
}
