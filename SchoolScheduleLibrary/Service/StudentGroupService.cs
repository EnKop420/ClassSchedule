using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Service.Interface;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class StudentGroupService : IStudentGroupService
    {
        private readonly IGenericRepository<StudentGroup> _StudentGroupGenericRepository;

        public StudentGroupService(IGenericRepository<StudentGroup> genericRepository)
        {
            _StudentGroupGenericRepository = genericRepository;
        }

        public async Task<StudentGroupDTO> CreateAsync(Guid institutionId, CreateStudentGroupDTO dto)
        {
            StudentGroup studentGroup = new(dto.Name, institutionId, dto.UserList);
            await _StudentGroupGenericRepository.Add(studentGroup);
            return new StudentGroupDTO(studentGroup.Id, studentGroup.Name, studentGroup.Users);
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {
            if (!await _StudentGroupGenericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find StudentGroup with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _StudentGroupGenericRepository.Delete(sg => sg.Id == id);
        }

        public async Task<List<StudentGroupDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _StudentGroupGenericRepository.GetAll())
                .Where(r => r.InstitutionId == institutionId)
                .Select(r => new StudentGroupDTO(r.Id, r.Name, r.Users)).ToList();
        }

        public async Task<StudentGroupDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            StudentGroup studentGroup = await _StudentGroupGenericRepository.Get(sg => sg.Id == id && sg.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get StudentGroup with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new StudentGroupDTO(studentGroup.Id, studentGroup.Name, studentGroup.Users);
        }

        public async Task<StudentGroupDTO> UpdateAsync(Guid institutionId, UpdateStudentGroupDTO dto)
        {
            StudentGroup studentGroup = await _StudentGroupGenericRepository.Get(sg => sg.Id == dto.Id && sg.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get StudentGroup with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            studentGroup.Name = dto.Name;

            StudentGroup updatedStudentGroup = await _StudentGroupGenericRepository.Update(studentGroup);

            return new StudentGroupDTO(updatedStudentGroup.Id, updatedStudentGroup.Name, updatedStudentGroup.Users);
        }
    }
}
