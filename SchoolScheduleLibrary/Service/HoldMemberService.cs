using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Enums;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Generic;
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

        public async Task<bool> GroupTeacherAsync(Guid institutionId, Guid holdId, Guid teacherId)
        {
            bool doesHoldExist = await _holdGenericRepository.DoesValueExist(h => h.Id == holdId && h.InstitutionId == institutionId);
            if (doesHoldExist == false) throw new NotFoundException($"Could not get Hold with Id \"{holdId}\" in the Institution with Id \"{institutionId}\"");

            User user = await _userGenericRepository.Get(u => u.Id == teacherId && u.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Teacher with Id \"{teacherId}\" in the Institution with Id \"{institutionId}\"");
            if (user.Role != UserRoles.Teacher) throw new BadRequestException("User is not a Teacher!");

            bool isTeacherGrouped = await _groupTeacherGenericRepository.DoesValueExist(e =>
                e.HoldId == holdId && e.TeacherId == teacherId);

            if (isTeacherGrouped) throw new BadRequestException("Teacher is already grouped into this hold");

            GroupTeacher groupTeacher = new(holdId, teacherId);

            return await _groupTeacherGenericRepository.Add(groupTeacher);
        }

        public async Task<bool> UngroupTeacherAsync(Guid institutionId, Guid holdId, Guid teacherId)
        {
            bool isTeacherGrouped = await _groupTeacherGenericRepository.DoesValueExist(e =>
                e.HoldId == holdId && e.TeacherId == teacherId);

            if (isTeacherGrouped == false) throw new BadRequestException("Teacher is not grouped into this hold");

            return await _groupTeacherGenericRepository.Delete(e => e.HoldId == holdId && e.TeacherId == teacherId);
        }

        public async Task<bool> EnrollStudentAsync(Guid institutionId, Guid holdId, Guid studentId)
        {
            bool doesHoldExist = await _holdGenericRepository.DoesValueExist(h => h.Id == holdId && h.InstitutionId == institutionId);
            if (doesHoldExist == false) throw new NotFoundException($"Could not get Hold with Id \"{holdId}\" in the Institution with Id \"{institutionId}\"");

            User user = await _userGenericRepository.Get(u => u.Id == studentId && u.InstitutionId == institutionId)
                ?? throw new NotFoundException($"Could not get Student with Id \"{studentId}\" in the Institution with Id \"{institutionId}\"");
            if (user.Role != UserRoles.Student) throw new BadRequestException("User is not a student!");

            bool isStudentEnrolled = await _enrollmentGenericRepository.DoesValueExist(e =>
                e.HoldId == holdId && e.StudentId == studentId);

            if (isStudentEnrolled) throw new BadRequestException("Student is already enrolled into this hold");

            Enrollment enrollment = new(holdId, studentId);

            return await _enrollmentGenericRepository.Add(enrollment);
        }

        public async Task<bool> UnenrollStudentAsync(Guid institutionId, Guid holdId, Guid studentId)
        {
            bool isStudentEnrolled = await _enrollmentGenericRepository.DoesValueExist(e =>
                e.HoldId == holdId && e.StudentId == studentId);

            if (isStudentEnrolled == false) throw new BadRequestException("Student is not enrolled into this hold");

            return await _enrollmentGenericRepository.Delete(e => e.HoldId == holdId && e.StudentId == studentId);
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
