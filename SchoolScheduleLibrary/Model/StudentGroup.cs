using SchoolScheduleLibrary.Model.Interface;

namespace SchoolScheduleLibrary.Model
{
    public class StudentGroup : IBaseEntity, IInstitutionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public List<User> Users { get; set; } = new();
        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;
        public StudentGroup(string name, Guid institutionId, List<User> users)
        {
            Name = name;
            InstitutionId = institutionId;
            Users = users;
        }
    }
}
