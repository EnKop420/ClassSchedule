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
        private readonly IStudentGroupMemberService _studentGroupMemberService;

        public StudentGroupService(
            IGenericRepository<StudentGroup> studentGroupGenericRepository,
            IGenericRepository<StudentGroupMember> studentGroupMemberGenericRepository,
            IStudentGroupMemberService studentGroupMemberService
        )
        {
            _StudentGroupGenericRepository = studentGroupGenericRepository;
            _studentGroupMemberService = studentGroupMemberService;
        }

        public async Task<StudentGroupDTO> CreateAsync(Guid institutionId, CreateStudentGroupDTO dto)
        {
            StudentGroup studentGroup = new(dto.Name, institutionId);

            if (!await _StudentGroupGenericRepository.Add(studentGroup))
            {
                throw new InternalErrorException("Could not create student group");
            }

            await _studentGroupMemberService.AddStudentListAsync(institutionId, studentGroup.Id, dto.StudentIds);

            return new StudentGroupDTO(studentGroup.Id, studentGroup.Name);
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
            return (await _StudentGroupGenericRepository.GetAll(sg => sg.InstitutionId == institutionId))
                .Select(sg => new StudentGroupDTO(sg.Id, sg.Name)).ToList();
        }

        public async Task<StudentGroupDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            StudentGroup studentGroup =await _StudentGroupGenericRepository.Get(sg => sg.Id == id && sg.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get StudentGroup with Id \"{id}\" in the Institution with Id \"{institutionId}\"");

            return new StudentGroupDTO(studentGroup.Id, studentGroup.Name);
        }

        public async Task<StudentGroupDTO> UpdateAsync(Guid institutionId, UpdateStudentGroupDTO dto)
        {
            StudentGroup studentGroup = await _StudentGroupGenericRepository.Get(sg => sg.Id == dto.Id && sg.InstitutionId == institutionId, sg => sg.Students)
                ?? throw new NotFoundException($"Could not get StudentGroup with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            studentGroup.Name = dto.Name;
            StudentGroup updatedStudentGroup = await _StudentGroupGenericRepository.Update(studentGroup);

            List<Guid> currentStudents = studentGroup.Students.Select(sgm => sgm.StudentId).ToList();

            await _studentGroupMemberService.RemoveStudentListAsync(institutionId, studentGroup.Id, currentStudents);

            List<Guid> students = dto.StudentIds.Distinct().ToList();
            await _studentGroupMemberService.AddStudentListAsync(institutionId, studentGroup.Id, students);

            return new StudentGroupDTO(updatedStudentGroup.Id, updatedStudentGroup.Name);
        }
    }
}