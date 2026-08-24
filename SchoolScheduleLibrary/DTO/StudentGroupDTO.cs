namespace SchoolScheduleLibrary.DTO
{
    public record StudentGroupDTO(Guid Id, string Name);
    public record CreateStudentGroupDTO(string Name, List<Guid> StudentIds);
    public record UpdateStudentGroupDTO(Guid Id, string Name, List<Guid> StudentIds);
}
