using SchoolScheduleLibrary.Model.Interface;

namespace SchoolScheduleLibrary.Model
{
    public class StudentGroup : IBaseEntity, IInstitutionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;
        public List<StudentGroupMember> Students { get; set; } = new();

        public StudentGroup(string name, Guid institutionId)
        {
            Name = name;
            InstitutionId = institutionId;
        }
    }
}
