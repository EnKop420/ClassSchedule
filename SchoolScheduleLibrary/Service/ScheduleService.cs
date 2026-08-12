using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service
{
    public class ScheduleService : IScheduleService
    {
        public Task<List<ScheduleLessonDTO>> GetScheduleAsync(GetScheduleLessonDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
