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
    public class LessonNoteService : ILessonNoteService
    {
        private readonly IGenericRepository<LessonNote> _lessonNoteGenericRepository;
        private readonly IGenericRepository<Lesson> _lessonGenericRepository;
        public LessonNoteService(IGenericRepository<LessonNote> lessonNoteGenericRepository, IGenericRepository<Lesson> lessonGenerciRepository)
        {
            _lessonNoteGenericRepository = lessonNoteGenericRepository;
            _lessonGenericRepository = lessonGenerciRepository;
        }

        public async Task<bool> AddNoteToLesson(CreateLessonNoteDTO dto)
        {
            Lesson lesson = await _lessonGenericRepository.Get(l => l.Id == dto.LessonId, l => l.Note!)
                ?? throw new NotFoundException($"Could not get Lesson with Id \"{dto.LessonId}\"");

            if (lesson.Note != null) throw new ConflictException("There is already a note attached to this Lesson!");

            lesson.Note = new(dto.LessonId, dto.AuthorId, dto.Content);
            if (lesson.IsModified == false) lesson.IsModified = true;

            return await _lessonGenericRepository.Update(lesson);
        }

        public async Task<bool> UpdateNoteFromLesson(UpdateLessonNoteDTO dto)
        {
            LessonNote note = await _lessonNoteGenericRepository.Get(n => n.Id == dto.Id)
                ?? throw new NotFoundException($"Could not get Lesson with Id \"{dto.Id}\"");

            note.Content = dto.Content;
            note.EditorId = dto.EditorId;
            note.LastEditedAt = DateTime.UtcNow;

            return await _lessonNoteGenericRepository.Update(note);
        }

        public async Task<bool> RemoveNoteFromLesson(Guid id)
        {
            if (!await _lessonNoteGenericRepository.DoesValueExist(t => t.Id == id))
            {
                throw new NotFoundException($"Could not find Note with Id \"{id}\"");
            }

            return await _lessonNoteGenericRepository.Delete(r => r.Id == id);
        }
    }
}
