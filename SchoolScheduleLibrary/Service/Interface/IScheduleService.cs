using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface IScheduleService
    {
        public Task<List<ScheduleLessonDTO>> GetScheduleAsync(GetScheduleLessonDTO dto);
    }
}
