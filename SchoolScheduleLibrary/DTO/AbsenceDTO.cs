using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.DTO
{
    public record AbsenceDTO(Guid LessonIds, List<Guid> StudentIds);
}
