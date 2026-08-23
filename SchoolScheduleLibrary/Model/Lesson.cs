using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class Lesson : IBaseEntity, IInstitutionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public LessonStatus Status { get; set; } = LessonStatus.Scheduled;
        public bool IsModified { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;

        public Guid HoldId { get; set; }
        public Hold Hold { get; set; } = null!;

        public Guid? TemplateId { get; set; }
        public LessonTemplate? Template { get; set; }

        public Guid? RoomId { get; set; }
        public Room? Room { get; set; }

        public List<LessonTeacher> Teachers { get; set; } = new();
    }
}
