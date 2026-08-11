using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        /**
         * Explaination of a Generic function with Reflection.
         * params : allows the user to pass comma seperated values that it then makes into an array instead of the user having to make the array.
         * Func : Just a delegate that takes and input (T) and returns and output (object).
         * Expression : An EF Core thing that tells the code not to compile the Func as a normal delegate which then allows EF to actually read the function.
         */
        public Task<T?> Get(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includes);

        public Task<List<T>> GetAll(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includes);
        
        public Task<bool> DoesValueExist(Expression<Func<T, bool>>? predicate = null);

        public Task<bool> Add(T entity);

        public Task<bool> AddRange(List<T> entities);

        public Task<T> Update(T entity);

        public Task<bool> Delete(Expression<Func<T, bool>> predicate);

        public Task<bool> RemoveRange(List<T> entities);
    }
}
