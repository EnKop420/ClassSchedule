using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Interface
{
    public interface IGenericRepository<T> where T : class, IBaseEntity
    {

        public Task<T?> GetById(Guid id);

        public Task<List<T>> GetAll();

        public Task<bool> DoesValueExist(Guid id);

        public Task<Guid> Create(T entity, bool returnId = true);

        public Task<bool> Insert(T entity);

        public Task<T> Update(T entity);

        public Task<bool> Delete(T entity);

        public Task<bool> DeleteById(Guid id);

    }
}
