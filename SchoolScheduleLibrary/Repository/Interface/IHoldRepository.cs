using SchoolScheduleLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    public interface IHoldRepository
    {
        public Task<Hold?> GetById(Guid institutionId, Guid id);
        public Task<List<Hold>> GetAll(Guid institutionId);
        public Task CheckTermAndSubjectForInstitution(Guid institutionId, Guid subjectId, Guid termId);
        public Task<Hold> Update(Hold hold);
    }
}
