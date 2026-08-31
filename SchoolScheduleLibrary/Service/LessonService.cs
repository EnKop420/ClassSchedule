using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
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
        private readonly IGenericRepository<Lesson> _lessonGenericRepository;

        public LessonService(
            ILessonRepository lessonRepository,
            IGenericRepository<Lesson> lessonGenericRepository
        )
        {
            _lessonRepository = lessonRepository;
            _lessonGenericRepository = lessonGenericRepository;
        }

        public async Task<List<MinimalUserInformationDTO>> GetStudentsFromSchedule(Guid id)
        {
            return (await _lessonRepository.GetStudentsFromLessonAsync(id))
                .Select(u => new MinimalUserInformationDTO($"{u.FirstName} {u.LastName}", u.Id)).ToList();
        }

        public async Task<bool> ChangeLessonStatus(Guid lessonId, LessonStatus status)
        {
            Lesson lesson = await _lessonGenericRepository.Get(l => l.Id == lessonId)
                ?? throw new NotFoundException($"Could not get Lesson with Id \"{lessonId}\"");

            lesson.Status = status;

            return await _lessonGenericRepository.Update(lesson);
        }
    }
}
