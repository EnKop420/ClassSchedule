using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class StudentGroupService : IStudentGroupService
    {
        private readonly IGenericRepository<StudentGroup> _StudentGroupGenericRepository;

        public StudentGroupService(IGenericRepository<StudentGroup> studentGroupGenericRepository)
        {
            _StudentGroupGenericRepository = studentGroupGenericRepository;
        }

        public async Task<bool> CreateAsync(Guid institutionId, CreateStudentGroupDTO dto)
        {
            StudentGroup studentGroup = new(dto.Name, institutionId);
            studentGroup.Students = dto.StudentIds.Distinct()
                .Select(s => new StudentGroupMember(studentGroup.Id, s))
                .ToList();

            return await _StudentGroupGenericRepository.Add(studentGroup);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _StudentGroupGenericRepository.DoesValueExist(t => t.Id == id))
            {
                throw new NotFoundException($"Could not find StudentGroup with Id \"{id}\"");
            }

            return await _StudentGroupGenericRepository.Delete(sg => sg.Id == id);
        }

        public async Task<List<StudentGroupDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _StudentGroupGenericRepository.GetAll(sg => sg.InstitutionId == institutionId))
                .Select(sg => new StudentGroupDTO(sg.Id, sg.Name)).ToList();
        }

        public async Task<StudentGroupDTO> GetByIdAsync(Guid id)
        {
            StudentGroup studentGroup =await _StudentGroupGenericRepository.Get(sg => sg.Id == id)
                ?? throw new NotFoundException($"Could not get StudentGroup with Id \"{id}\"");

            return new StudentGroupDTO(studentGroup.Id, studentGroup.Name);
        }

        public async Task<bool> UpdateAsync(UpdateStudentGroupDTO dto)
        {
            StudentGroup studentGroup = await _StudentGroupGenericRepository.Get(sg => sg.Id == dto.Id, sg => sg.Students)
                ?? throw new NotFoundException($"Could not get StudentGroup with Id \"{dto.Id}\"");

            studentGroup.Name = dto.Name;
            studentGroup.Students = dto.StudentIds.Distinct()
                .Select(s => new StudentGroupMember(studentGroup.Id, s))
                .ToList();

            return await _StudentGroupGenericRepository.Update(studentGroup);
        }
    }
}