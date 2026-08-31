using SchoolScheduleLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Service.Interface
{
    public interface ILessonNoteService
    {
        public Task<bool> AddNoteToLesson(Guid authorId, CreateLessonNoteDTO dto);
        public Task<bool> UpdateNoteFromLesson(Guid editorId, UpdateLessonNoteDTO dto);
        public Task<bool> RemoveNoteFromLesson(Guid noteId);
    }
}
