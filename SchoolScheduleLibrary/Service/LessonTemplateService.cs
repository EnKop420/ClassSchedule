using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository;
using SchoolScheduleLibrary.Repository.Generic;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolScheduleLibrary.Service
{
    public class LessonTemplateService : ILessonTemplateService
    {
        private readonly IGenericRepository<LessonTemplate> _lessonTemplateGenericRepository;
        private readonly IGenericRepository<Hold> _holdGenericRepository;
        private readonly IGenericRepository<Period> _periodGenericRepository;
        private readonly IGenericRepository<Room> _roomGenericRepository;
        public LessonTemplateService(
            IGenericRepository<LessonTemplate> lessonTemplateGenericRepository,
            IGenericRepository<Hold> holdGenericRepository,
            IGenericRepository<Period> periodGenericRepository,
            IGenericRepository<Room> roomGenericRepository
        )
        {
            _lessonTemplateGenericRepository = lessonTemplateGenericRepository;
            _holdGenericRepository = holdGenericRepository;
            _periodGenericRepository = periodGenericRepository;
            _roomGenericRepository = roomGenericRepository;
        }

        public async Task<List<LessonTemplateDTO>> GetAllAsync(Guid institutionId)
        {
            return (await _lessonTemplateGenericRepository.GetAll(
                lt => lt.InstitutionId == institutionId,
                lt => lt.Period,
                lt => lt.Room!,
                lt => lt.Hold)
            )
            .Select(
                lt => new LessonTemplateDTO(
                lt.Id, 
                lt.WeekDay, 
                lt.ValidFrom, 
                lt.ValidTo, 
                lt.Hold.Name, 
                lt.Period.Name, 
                lt.Room?.Name
            )).ToList();
        }

        public async Task<LessonTemplateDTO> GetByIdAsync(Guid institutionId, Guid id)
        {
            LessonTemplate lessonTemplate = await _lessonTemplateGenericRepository.GetById(
                lt => lt.Id == id && lt.InstitutionId == institutionId,
                lt => lt.Period,
                lt => lt.Room!,
                lt => lt.Hold)
                ?? throw new NotFoundException($"LessonTemplate with ID {id} does not exist in the Institution with the Id {institutionId}");

            return new LessonTemplateDTO(
                lessonTemplate.Id,
                lessonTemplate.WeekDay,
                lessonTemplate.ValidFrom,
                lessonTemplate.ValidTo,
                lessonTemplate.Hold.Name,
                lessonTemplate.Period.Name,
                lessonTemplate.Room?.Name
            );
        }

        public async Task<LessonTemplateDTO> CreateAsync(Guid institutionId, CreateLessonTemplateDTO dto)
        {
            // Check if weekday is a valid number.
            if (dto.WeekDay < 1 || dto.WeekDay > 7)
                throw new BadRequestException("WeekDay must be 1 (Mon) through 7 (Sun).");

            // Check dates are valid.
            if (dto.ValidFrom > dto.ValidTo) throw new BadRequestException("Valid From has to be before Valid To!");

            Period period = await _periodGenericRepository.GetById(p => p.Id == dto.PeriodId && p.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Period with Id \"{dto.PeriodId}\" in the Institution with Id \"{institutionId}\"");

            Hold hold = await _holdGenericRepository.GetById(h => h.Id == dto.HoldId && h.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Hold with Id \"{dto.HoldId}\" in the Institution with Id \"{institutionId}\"");

            Room? room = null;

            if (dto.RoomId != null)
            {
                room = await _roomGenericRepository.GetById(r => r.Id == dto.RoomId && r.InstitutionId == institutionId)
                    ?? throw new NotFoundException($"Could not get Room with Id \"{dto.RoomId}\" in the Institution with Id \"{institutionId}\"");
            }

            if (await _lessonTemplateGenericRepository.DoesValueExist(
                lt => lt.HoldId == dto.HoldId
                && lt.PeriodId == dto.PeriodId
                && lt.WeekDay == dto.WeekDay
                && lt.ValidFrom == dto.ValidFrom 
                && lt.ValidTo == dto.ValidTo
                && lt.InstitutionId == institutionId)
            ) throw new BadRequestException("This exact template with the same data!");

            LessonTemplate lessonTemplate = new(dto.WeekDay, dto.ValidFrom, dto.ValidTo, dto.PeriodId, dto.RoomId, dto.HoldId, institutionId);

            await _lessonTemplateGenericRepository.Create(lessonTemplate);

            return new LessonTemplateDTO(
                lessonTemplate.Id,
                lessonTemplate.WeekDay,
                lessonTemplate.ValidFrom,
                lessonTemplate.ValidTo,
                hold.Name,
                period.Name,
                room?.Name
            );
        }
        public async Task<LessonTemplateDTO> UpdateAsync(Guid institutionId, UpdateLessonTemplateDTO dto)
        {
            // Check if weekday is a valid number.
            if (dto.WeekDay < 1 || dto.WeekDay > 7)
                throw new BadRequestException("WeekDay must be 1 (Mon) through 7 (Sun).");

            // Check dates are valid.
            if (dto.ValidFrom > dto.ValidTo) throw new BadRequestException("Valid From has to be before Valid To!");

            LessonTemplate lessonTemplate = await _lessonTemplateGenericRepository.GetById(h => h.Id == dto.Id && h.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get LessonTemplate with Id \"{dto.Id}\" in the Institution with Id \"{institutionId}\"");

            // Check period, room and hold are valid.
            if(!await _periodGenericRepository.DoesValueExist(p => p.InstitutionId == institutionId && p.Id == dto.PeriodId))
                throw new NotFoundException($"Could not find Period with Id \"{dto.PeriodId}\" in the Institution with Id \"{institutionId}\"");

            if(!await _holdGenericRepository.DoesValueExist(h => h.InstitutionId == institutionId && h.Id == dto.HoldId))
                throw new NotFoundException($"Could not find Hold with Id \"{dto.HoldId}\" in the Institution with Id \"{institutionId}\"");

            if (dto.RoomId != null)
            {
                if(!await _roomGenericRepository.DoesValueExist(r => r.InstitutionId == institutionId && r.Id == dto.RoomId))
                    throw new NotFoundException($"Could not find Room with Id \"{dto.RoomId}\" in the Institution with Id \"{institutionId}\"");
            }

            if (await _lessonTemplateGenericRepository.DoesValueExist(
                lt => lt.Id == dto.Id
                && lt.HoldId == dto.HoldId
                && lt.PeriodId == dto.PeriodId
                && lt.WeekDay == dto.WeekDay
                && lt.ValidFrom == dto.ValidFrom
                && lt.ValidTo == dto.ValidTo
                && lt.RoomId == dto.RoomId
                && lt.InstitutionId == institutionId)
            ) throw new BadRequestException("This exact template with the same data!");

            lessonTemplate.WeekDay = dto.WeekDay;
            lessonTemplate.ValidFrom = dto.ValidFrom;
            lessonTemplate.ValidTo = dto.ValidTo;
            lessonTemplate.PeriodId = dto.PeriodId;
            lessonTemplate.RoomId = dto.RoomId;
            lessonTemplate.HoldId = dto.HoldId;

            await _lessonTemplateGenericRepository.Update(lessonTemplate);

            LessonTemplate updatedLessonTemplate = await _lessonTemplateGenericRepository.GetById(
                lt => lt.Id == dto.Id && lt.InstitutionId == institutionId, // Predicate
                lt => lt.Period, // Include
                lt => lt.Room!, // Include
                lt => lt.Hold // Include
            ) ?? throw new InternalErrorException("Something went wrong after updating and could not retrieve it!");

            return new LessonTemplateDTO(
                lessonTemplate.Id,
                lessonTemplate.WeekDay,
                lessonTemplate.ValidFrom,
                lessonTemplate.ValidTo,
                lessonTemplate.Hold.Name,
                lessonTemplate.Period.Name,
                lessonTemplate.Room?.Name
            );
        }

        public async Task<bool> DeleteAsync(Guid institutionId, Guid id)
        {

            if (!await _lessonTemplateGenericRepository.DoesValueExist(t => t.Id == id && t.InstitutionId == institutionId))
            {
                throw new NotFoundException($"Could not find LessonTemplate with Id \"{id}\" in the Institution with Id \"{institutionId}\"");
            }

            return await _lessonTemplateGenericRepository.DeleteById(id);
        }
    }
}
