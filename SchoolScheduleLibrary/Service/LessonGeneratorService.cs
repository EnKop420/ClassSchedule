using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Service
{
    public class LessonGeneratorService : ILessonGenerationService
    {
        private readonly IGenericRepository<Term> _termGenericRepository;
        private readonly IGenericRepository<Hold> _holdGenericRepository;
        private readonly IGenericRepository<LessonTemplate> _lessonTemplateGenericRepository;
        private readonly IGenericRepository<Period> _periodGenericRepository;
        private readonly IGenericRepository<NonTeachingDay> _nonTeachingDayGenericRepository;
        private readonly IGroupTeacherRepository _groupTeacherGenericRepository;
        private readonly IGenericRepository<Lesson> _lessonGenericRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonGeneratorService()
        {   
        }

        public async Task<int> GenerateForTermAsync(Guid institutionId, Guid termId)
        {
            throw new NotImplementedException();
        }

    }
}
