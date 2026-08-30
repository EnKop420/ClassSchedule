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
        private readonly IGenericRepository<GroupTeacher> _groupTeacherGenericRepository;
        private readonly IGenericRepository<Enrollment> _enrollmentGenericRepository;

        public HoldMemberService(
            IGenericRepository<GroupTeacher> groupTeacherGenericRepository,
            IGenericRepository<Enrollment> enrollmentGenericRepository)
        {
            _groupTeacherGenericRepository = groupTeacherGenericRepository;
            _enrollmentGenericRepository = enrollmentGenericRepository;
        }

        public async Task<List<MinimalUserInformationDTO>> GetTeachersAsync(Guid holdId)
        {
            return (await _groupTeacherGenericRepository.GetAll(gt => gt.HoldId == holdId, gt => gt.Teacher))
                .Select(t => new MinimalUserInformationDTO($"{t.Teacher.FirstName} {t.Teacher.LastName}", t.TeacherId)).ToList();
        }

        public async Task<List<MinimalUserInformationDTO>> GetStudentsAsync(Guid holdId)
        {
            return (await _enrollmentGenericRepository.GetAll(es => es.HoldId == holdId, es => es.Student))
                .Select(t => new MinimalUserInformationDTO($"{t.Student.FirstName} {t.Student.LastName}", t.StudentId)).ToList();
        }
    }
}
