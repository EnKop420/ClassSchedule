using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository
{
    public class GroupTeacherRepository : IGroupTeacherRepository
    {
        private readonly SchoolDbContext _context;
        public GroupTeacherRepository(SchoolDbContext context)
        {
            _context = context;
        }

        public Task<GroupTeacher> Create(GroupTeacher groupTeacher)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GroupTeacher?> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GroupTeacher> Update(GroupTeacher groupTeacher)
        {
            throw new NotImplementedException();
        }
    }
}
