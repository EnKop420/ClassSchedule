using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    public interface IGroupTeacherRepository
    {
        public Task<GroupTeacher?> GetById(Guid id);
        public Task<GroupTeacher> Create(GroupTeacher groupTeacher);
        public Task<GroupTeacher> Update(GroupTeacher groupTeacher);
        public Task Delete(Guid id);
    }
}
