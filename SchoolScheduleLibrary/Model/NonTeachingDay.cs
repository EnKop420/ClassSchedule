using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    // " = null!;" is used to tell the code that it should by default be set to null but still treat it as a non null variable. Used mostly to supress the warnings.
    // EF Core will fill in the value later.
    public class NonTeachingDay : IBaseEntity, IInstitutionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateOnly Date {  get; set; }
        public string Reason { get; set; }

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = null!;
    }
}
