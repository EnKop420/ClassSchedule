using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<List<MinimalUserInformationDTO>> GetStudentsFromSchedule(Guid id)
        {
            return (await _lessonRepository.GetStudentsFromLessonAsync(id))
                .Select(u => new MinimalUserInformationDTO($"{u.FirstName} {u.LastName}", u.Id)).ToList();
        }
    }
}
