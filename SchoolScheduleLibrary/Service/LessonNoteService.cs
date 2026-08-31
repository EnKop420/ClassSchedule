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
        private readonly IGenericRepository<User> _userGenericRepository;
        public LessonNoteService(
            IGenericRepository<LessonNote> lessonNoteGenericRepository,
            IGenericRepository<Lesson> lessonGenerciRepository,
            IGenericRepository<User> userGenericRepository
        )
        {
            _lessonNoteGenericRepository = lessonNoteGenericRepository;
            _lessonGenericRepository = lessonGenerciRepository;
            _userGenericRepository = userGenericRepository;
        }

        public async Task<bool> AddNoteToLesson(Guid authorId, CreateLessonNoteDTO dto)
        {
            if (!await _userGenericRepository.DoesValueExist(u => u.Id == authorId))
            {
                throw new NotFoundException($"User could not be found with Author Id \"{authorId}\"");
            }

            Lesson lesson = await _lessonGenericRepository.Get(l => l.Id == dto.LessonId, l => l.Note!)
                ?? throw new NotFoundException($"Could not get Lesson with Id \"{dto.LessonId}\"");

            if (lesson.Note != null) throw new ConflictException("There is already a note attached to this Lesson!");

            LessonNote note = new(dto.LessonId, authorId, dto.Content);

            if (lesson.IsModified == false) lesson.IsModified = true;

            if (await _lessonNoteGenericRepository.Add(note))
            {
                return await _lessonGenericRepository.Update(lesson);
            }
            else throw new InternalErrorException("Something went wrong when adding the note!");
        }

        public async Task<bool> UpdateNoteFromLesson(Guid editorId, UpdateLessonNoteDTO dto)
        {
            if (!await _userGenericRepository.DoesValueExist(u => u.Id == editorId))
            {
                throw new NotFoundException($"User could not be found with Editor Id \"{editorId}\"");
            }

            LessonNote note = await _lessonNoteGenericRepository.Get(n => n.Id == dto.Id)
                ?? throw new NotFoundException($"Could not get Lesson with Id \"{dto.Id}\"");

            note.Content = dto.Content;
            note.EditorId = editorId;
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
