namespace SchoolScheduleLibrary.DTO
{
    public record StudentGroupMemberDTO(string StudentName, Guid StudentId);
    public record StudentGroupDTO(Guid Id, string Name, List<StudentGroupMemberDTO> Students);
    public record CreateStudentGroupDTO(string Name, List<Guid> StudentIds);
    public record UpdateStudentGroupDTO(Guid Id, string Name);
}
