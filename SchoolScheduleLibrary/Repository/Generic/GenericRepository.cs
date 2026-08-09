using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Utilities.Encryption;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SchoolScheduleLibrary.Repository.Generic
{
    // T can be any type, as long as it inherits from BaseEntity. This ensures that T has an Id property of type Guid.
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
    {
        private readonly SchoolDbContext _context;

        public GenericRepository(SchoolDbContext context, IEncryptionHandler encryptionHandler)
        {
            _context = context;
        }

        public async Task<T?> GetById(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> DoesValueExist(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.AnyAsync();
        }

        public async Task<Guid> Create(T entity, bool returnId = true)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            if (returnId) return entity.Id;
            else return Guid.Empty;
        }

        public async Task<bool> Delete(T entity)
        {
            return await _context.Set<T>().Where(e => e.Id == entity.Id).ExecuteDeleteAsync() > 0;
        }

        public async Task<bool> DeleteById(Guid id)
        {
            return await _context.Set<T>().Where(e => e.Id == id).ExecuteDeleteAsync() > 0;
        }

        public async Task<T> Update(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Insert(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
