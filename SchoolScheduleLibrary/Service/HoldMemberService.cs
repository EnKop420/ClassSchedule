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
    public class HoldMemberService : IHoldMemberService
    {
        private readonly IGenericRepository<Hold> _holdGenericRepository;
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IGenericRepository<GroupTeacher> _groupTeacherGenericRepository;
        private readonly IGenericRepository<Enrollment> _enrollmentGenericRepository;

        public HoldMemberService(
            IGenericRepository<Hold> holdGenericRepository,
            IGenericRepository<User> userGenericRepository,
            IGenericRepository<GroupTeacher> groupTeacherGenericRepository,
            IGenericRepository<Enrollment> enrollmentGenericRepository)
        {
            _holdGenericRepository = holdGenericRepository;
            _userGenericRepository = userGenericRepository;
            _groupTeacherGenericRepository = groupTeacherGenericRepository;
            _enrollmentGenericRepository = enrollmentGenericRepository;
        }

        public async Task<bool> GroupTeacherAsync(Guid institutionId, Guid holdId, List<Guid> teacherIds)
        {
            bool doesHoldExist = await _holdGenericRepository.DoesValueExist(h => h.Id == holdId && h.InstitutionId == institutionId);
            if (doesHoldExist == false) throw new NotFoundException($"Could not get Hold with Id \"{holdId}\" in the Institution with Id \"{institutionId}\"");

            List<GroupTeacher> groupTeachers = [];

            foreach (Guid teacher in teacherIds)
            {
                User user = await _userGenericRepository.Get(u => u.Id == teacher && u.InstitutionId == institutionId)
                    ?? throw new NotFoundException($"Could not get Teacher with Id \"{teacher}\" in the Institution with Id \"{institutionId}\"");
                if (user.Role != UserRoles.Teacher) throw new BadRequestException("User is not a Teacher!");

                bool isTeacherGrouped = await _groupTeacherGenericRepository.DoesValueExist(e =>
                    e.HoldId == holdId && e.TeacherId == teacher);

                if (isTeacherGrouped)
                {
                    throw new ConflictException($"{user.FirstName} {user.LastName} {teacher} Teacher is already grouped into this hold");
                }
                else
                {
                    groupTeachers.Add(new(holdId, teacher));
                }
            }

            return await _groupTeacherGenericRepository.AddRange(groupTeachers);
        }

        public async Task<bool> UngroupTeacherAsync(Guid institutionId, Guid holdId, List<Guid> teacherIds)
        {
            int groupedTeachers = await _groupTeacherGenericRepository.Count(tg =>
                tg.HoldId == holdId && teacherIds.Contains(tg.TeacherId));

            if (groupedTeachers != teacherIds.Distinct().Count()) throw new BadRequestException("One or more teacher(s) is not grouped into this hold");

            return await _groupTeacherGenericRepository.Delete(tg => tg.HoldId == holdId && teacherIds.Contains(tg.TeacherId));
        }

        public async Task<bool> EnrollStudentAsync(Guid institutionId, Guid holdId, List<Guid> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0) return false;

            bool doesHoldExist = await _holdGenericRepository.DoesValueExist(h => h.Id == holdId && h.InstitutionId == institutionId);
            if (doesHoldExist == false) throw new NotFoundException($"Could not get Hold with Id \"{holdId}\" in the Institution with Id \"{institutionId}\"");

            List<Enrollment> enrollments = [];

            foreach (Guid student in studentIds)
            {
                User user = await _userGenericRepository.Get(u => u.Id == student && u.InstitutionId == institutionId)
                    ?? throw new NotFoundException($"Could not get Student with Id \"{studentIds}\" in the Institution with Id \"{institutionId}\"");
                if (user.Role != UserRoles.Student) throw new BadRequestException("User is not a student!");

                bool isStudentEnrolled = await _enrollmentGenericRepository.DoesValueExist(e =>
                    e.HoldId == holdId && e.StudentId == student);

                if (isStudentEnrolled)
                {
                    throw new ConflictException($"{user.FirstName} {user.LastName} {student} Student is already enrolled into this hold");
                }
                else
                {
                    enrollments.Add(new(holdId, student));
                }
            }

            return await _enrollmentGenericRepository.AddRange(enrollments);
        }

        public async Task<bool> UnenrollStudentAsync(Guid institutionId, Guid holdId, List<Guid> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0) return false;

            int enrolledStudents = await _enrollmentGenericRepository.Count(e =>
                e.HoldId == holdId && studentIds.Contains(e.StudentId));

            if (enrolledStudents != studentIds.Distinct().Count()) throw new BadRequestException("One or more student(s) is not enrolled into this hold");

            return await _enrollmentGenericRepository.Delete(e => e.HoldId == holdId && studentIds.Contains(e.StudentId));
        }

        public async Task<List<HoldMemberDTO>> GetTeachersAsync(Guid holdId)
        {
            List<HoldMemberDTO> teachers = (await _groupTeacherGenericRepository.GetAll(gt => gt.HoldId == holdId, gt => gt.Teacher))
                .Select(t => new HoldMemberDTO($"{t.Teacher.FirstName} {t.Teacher.LastName}", t.TeacherId, holdId)).ToList();

            return teachers;
        }

        public async Task<List<HoldMemberDTO>> GetStudentsAsync(Guid holdId)
        {
            List<HoldMemberDTO> students = (await _enrollmentGenericRepository.GetAll(es => es.HoldId == holdId, es => es.Student))
                .Select(t => new HoldMemberDTO($"{t.Student.FirstName} {t.Student.LastName}", t.StudentId, holdId)).ToList();

            return students;
        }
    }
}
