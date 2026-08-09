using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class LessonTemplate : IBaseEntity, IInstitutionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid HoldId { get; set; }
        public Hold Hold { get; set; } = null!;
        public int WeekDay { get; set; } // ISO 1=Mon .. 7=Sun
        public Guid PeriodId { get; set; }
        public Period Period { get; set; } = null!;
        public Guid? RoomId { get; set; } // Room is optional
        public Room? Room { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly ValidTo { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;
        public List<Lesson> Lessons { get; set; } = new();

        public LessonTemplate(int weekDay, DateOnly validFrom, DateOnly validTo, Guid periodId, Guid? roomId, Guid holdId, Guid institutionId)
        {
            WeekDay = weekDay;
            ValidFrom = validFrom;
            ValidTo = validTo;
            PeriodId = periodId;
            RoomId = roomId;
            HoldId = holdId;
            InstitutionId = institutionId;
        }
    }
}
