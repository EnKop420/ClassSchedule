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

        public async Task<T?> GetByGuid(Guid id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e =>
                e.Id == id
            );
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
            if (_context.Set<T>().Remove(entity) != null)
            {
                return await _context.SaveChangesAsync() > 0;
            }
            else return false;
        }

        public async Task<bool> DeleteById(Guid id)
        {
            if (await _context.Set<T>().Where(e => e.Id == id).ExecuteDeleteAsync() > 0)
            {
                return await _context.SaveChangesAsync() > 0;
            }
            else return false;
        }

        public async Task<bool> Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Insert(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DoesUsernameExist<TUser>(string username) where TUser : class, IUser
        {
            return await _context.Set<TUser>().AnyAsync(u => u.Username == username);
        }

        public async Task<IUser> Login<TUser>(LoginDTO loginDTO) where TUser : class, IUser
        {
            Guid userId = Guid.Empty;
            string unauthenticatedMessage = "No match found for username and password!";

            TUser? user = await _context.Set<TUser>().FirstOrDefaultAsync(u => u.Username == loginDTO.Username && u.Password == loginDTO.Password);
            if (user == null) throw new HttpResponseException(ServiceReturnCode.Forbidden, unauthenticatedMessage);
            else return user;
        }
    }
}
