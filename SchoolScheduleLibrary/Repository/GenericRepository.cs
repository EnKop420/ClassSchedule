using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.DTO;
using SchoolScheduleLibrary.Model.Interface;
using SchoolScheduleLibrary.Repository.Interface;
using SchoolScheduleLibrary.Utilities.Encryption;
using SchoolScheduleLibrary.Utilities.Encryption.Interface;
using SchoolScheduleLibrary.Utilities.Response;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SchoolScheduleLibrary.Repository
{
    // T can be any type, as long as it inherits from BaseEntity. This ensures that T has an Id property of type Guid.
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
    {
        private readonly SchoolDbContext _context;

        public GenericRepository(SchoolDbContext context, IEncryptionHandler encryptionHandler)
        {
            _context = context;
        }

        public async Task<T?> GetById(Guid id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e =>
                e.Id == id
            );
        }

        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<bool> DoesValueExist(Guid id)
        {
            return await _context.Set<T>().AnyAsync(x => x.Id == id);
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
