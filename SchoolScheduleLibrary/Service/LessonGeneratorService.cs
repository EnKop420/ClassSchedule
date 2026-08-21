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
    public class LessonGeneratorService : ILessonGenerationService
    {
        private readonly IGenericRepository<Term> _termGenericRepository;
        private readonly IGenericRepository<Hold> _holdGenericRepository;
        private readonly IGenericRepository<LessonTemplate> _lessonTemplateGenericRepository;
        private readonly IGenericRepository<Period> _periodGenericRepository;
        private readonly IGenericRepository<NonTeachingDay> _nonTeachingDayGenericRepository;
        private readonly IGenericRepository<GroupTeacher> _groupTeacherGenericRepository;
        private readonly IGenericRepository<Lesson> _lessonGenericRepository;

        public LessonGeneratorService(
            IGenericRepository<Term> termGenericRepository,
            IGenericRepository<Hold> holdGenericRepository,
            IGenericRepository<LessonTemplate> lessonTemplateGenericRepository,
            IGenericRepository<Period> periodGenericRepository,
            IGenericRepository<NonTeachingDay> nonTeachingDayGenericRepository,
            IGenericRepository<GroupTeacher> groupTeacherGenericRepository,
            IGenericRepository<Lesson> lessonGenericRepository)
        {
            _termGenericRepository = termGenericRepository;
            _holdGenericRepository = holdGenericRepository;
            _lessonTemplateGenericRepository = lessonTemplateGenericRepository;
            _periodGenericRepository = periodGenericRepository;
            _nonTeachingDayGenericRepository = nonTeachingDayGenericRepository;
            _groupTeacherGenericRepository = groupTeacherGenericRepository;
            _lessonGenericRepository = lessonGenericRepository;
        }

        public async Task<int> GenerateForTermAsync(Guid institutionId, Guid termId)
        {
            Term term = await _termGenericRepository.Get(t => t.Id == termId && t.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Term with Id \"{termId}\" in the Institution with Id \"{institutionId}\"");

            // Holds in this term.
            List<Hold> holds = await _holdGenericRepository.GetAll(h => h.TermId == termId);
            HashSet<Guid> holdIds = holds.Select(h => h.Id).ToHashSet();

            // Templates hang off holds. Filter by Hold
            List<LessonTemplate> lessonTemplates = 
                await _lessonTemplateGenericRepository.GetAll(lt => holdIds.Contains(lt.HoldId));

            // Period times keyed by id for quick lookup
            Dictionary<Guid, Period> periods = 
                (await _periodGenericRepository.GetAll(p => p.InstitutionId == institutionId)).ToDictionary(p => p.Id);

            // Non teaching days in this term. Used for dates to skip.
            // HashSets gurantees uniques and faster lookup.
            List<NonTeachingDay> ranges = await _nonTeachingDayGenericRepository.GetAll(ntd =>
                ntd.InstitutionId == institutionId
                && ntd.StartDate <= term.EndDate
                && ntd.EndDate >= term.StartDate);

            HashSet<DateOnly> nonTeachingDays = new();
            foreach (NonTeachingDay r in ranges)
            {
                for (DateOnly d = r.StartDate; d <= r.EndDate; d = d.AddDays(1))
                {
                    nonTeachingDays.Add(d);
                }
            }

            // Each holds primary teachers. Grouped by Hold
            Dictionary<Guid, List<GroupTeacher>> teachersByHold =
                (await _groupTeacherGenericRepository.GetAll(gt => holdIds.Contains(gt.HoldId)))
                .GroupBy(gt => gt.HoldId)
                .ToDictionary(g => g.Key, g => g.ToList());

            //// Lessons that already exist. Ensures no duplicates.
            //HashSet<(Guid, DateOnly)> existing =
            //    (await _lessonGenericRepository.GetAll(l => holdIds.Contains(l.HoldId)))
            //    .Where(l => l.TemplateId != null)
            //    .Select(l => (l.TemplateId!.Value, l.Date))
            //    .ToHashSet();

            // All old auto generated lessons that hasnt been modified.
            List<Lesson> deletable =
                (await _lessonGenericRepository.GetAll(l => 
                    holdIds.Contains(l.HoldId)
                    && l.TemplateId != null
                    && l.IsModified == false))
                .ToList();

            // Existing lessons that has been modified by a person.
            HashSet<(Guid, DateOnly)> keep =
                (await _lessonGenericRepository.GetAll(l =>
                    holdIds.Contains(l.HoldId)
                    && l.TemplateId != null
                    && l.IsModified == true))
                .Select(l => (l.TemplateId!.Value, l.Date))
                .ToHashSet();

            List<Lesson> newLessons = new();

            foreach (LessonTemplate lt in lessonTemplates)
            {
                // Restrict the templates active period so it stays strictly within the terms start and end dates
                DateOnly from = lt.ValidFrom > term.StartDate ? lt.ValidFrom : term.StartDate;
                DateOnly to = lt.ValidTo < term.EndDate ? lt.ValidTo : term.EndDate;

                for (DateOnly d = from; d <= to; d= d.AddDays(1))
                {
                    // .NET DayOfWeek: Sunday=0..Saturday=6 -> ISO Monday=1..Sunday=7
                    int isoDay = d.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)d.DayOfWeek;

                    if (isoDay != lt.WeekDay) continue; // wrong weekday
                    if (nonTeachingDays.Contains(d)) continue; // non-teaching day
                    if (keep.Contains((lt.Id, d))) continue; // Human modified data

                    if (!periods.TryGetValue(lt.PeriodId, out Period? period))
                        throw new BadRequestException($"Lesson Template {lt.Id} points at a missing period!");

                    Lesson lesson = new()
                    {
                        InstitutionId = institutionId,
                        HoldId = lt.HoldId,
                        TemplateId = lt.Id,
                        Date = d,
                        StartTime = period.StartTime,   // copied from the period
                        EndTime = period.EndTime,
                        RoomId = lt.RoomId,
                        Status = LessonStatus.Scheduled
                    };

                    // Set the holds default teachers as the primary teacher
                    if (teachersByHold.TryGetValue(lt.HoldId, out List<GroupTeacher>? gts))
                    {
                        foreach (GroupTeacher g in gts)
                        {
                            lesson.Teachers.Add(new LessonTeacher
                            {
                                TeacherId = g.TeacherId,
                                Role = TeacherRole.Primary
                            });
                        }
                    }

                    newLessons.Add(lesson);
                }
            }

            await _lessonGenericRepository.RemoveRange(deletable);

            if (await _lessonGenericRepository.AddRange(newLessons) == false) throw new InternalErrorException("No lessons was added to the database!");

            return newLessons.Count;
        }
    }
}
