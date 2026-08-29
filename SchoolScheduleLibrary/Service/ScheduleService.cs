using Microsoft.Extensions.DependencyModel.Resolution;
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
    public class ScheduleService : IScheduleService
    {
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly ILessonRepository _scheduleRepository;
        public ScheduleService(IGenericRepository<User> userGenericRepository, ILessonRepository scheduleRepository)
        {
            _userGenericRepository = userGenericRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<List<ScheduleLessonDTO>> GetScheduleAsync(Guid institutionId, Guid callerId, UserRoles callerRole, GetScheduleLessonDTO dto)
        {
            if (dto.From > dto.To) throw new BadRequestException("'From' needs to be on or before 'To'");
            if (dto.To.DayNumber - dto.From.DayNumber > 62) throw new BadRequestException("Range too large.");

            switch (callerRole)
            {
                case UserRoles.Student:
                    if (dto.TargetId != callerId)
                    {
                        throw new UnauthorizedException("Students can only view their own schedule!");
                    }
                    break;

                case UserRoles.Admin:
                    if (dto.TargetId == callerId)
                    {
                        throw new UnauthorizedException("Admins don't have a personal schedule!");
                    }
                    break;

                case UserRoles.Teacher:
                    break; // Teachers have no restriction.
            }

            User target = await _userGenericRepository.Get(u => u.Id == dto.TargetId && u.InstitutionId == institutionId)
                ?? throw new NotFoundException("Target user could not be found in this Institution!");

            List<Lesson> lessons = target.Role switch
            {
                UserRoles.Student => await _scheduleRepository.GetStudentLessonsAsync(institutionId, dto),
                UserRoles.Teacher => await _scheduleRepository.GetTeacherLessonsAsync(institutionId, dto),
                _ => throw new NotFoundException("That user has no schedule!")
            };

            return lessons.Select(l => new ScheduleLessonDTO(
                l.Id, l.Date, l.StartTime, l.EndTime,
                l.Hold.Subject.Name,
                l.Hold.Name,
                l.Room?.Name,
                l.Status.ToString(),
                l.Teachers.Select(t => new MinimalUserInformationDTO(
                    $"{t.Teacher.FirstName} {t.Teacher.LastName}",
                    t.TeacherId)
                ).ToList()
            )).ToList();
        }
    }
}
