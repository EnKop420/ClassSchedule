using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface ILessonGenerationService
    {
        public Task<int> GenerateForTermAsync(Guid institutionId, Guid termId);
    }
}
