using Microsoft.EntityFrameworkCore;
using SchoolScheduleLibrary.Context;
using SchoolScheduleLibrary.Model;
using SchoolScheduleLibrary.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using static SchoolScheduleLibrary.Utilities.Response.HttpResponseException;

namespace SchoolScheduleLibrary.Repository
{
    public class HoldRepository : IHoldRepository
    {
        private readonly SchoolDbContext _context;
        public HoldRepository(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<Hold?> GetById(Guid institutionId, Guid id)
        {
            return await _context.Holds
                .Include(h => h.Term)
                .Include(h => h.Subject)
                .FirstOrDefaultAsync(h => h.Id == id && h.InstitutionId == institutionId);
        }

        public async Task<List<Hold>> GetAll(Guid institutionId)
        {
            return await _context.Holds
                .Where(h => h.InstitutionId == institutionId)
                .Include(h => h.Subject)
                .Include(h => h.Term)
                .ToListAsync();
        }

        public async Task CheckTermAndSubjectForInstitution(Guid institutionId, Guid subjectId, Guid termId)
        {
            bool doesSubjectExist = await _context.Subjects.AnyAsync(s => s.Id == subjectId && s.InstitutionId == institutionId);
            bool doesTermExist = await _context.Terms.AnyAsync(t => t.Id == termId && t.InstitutionId == institutionId);

            if (doesSubjectExist == false) throw new NotFoundException($"Invalid Subject. No subject exists with id {subjectId} with Institution Id {institutionId}");
            else if (doesTermExist == false) throw new NotFoundException($"Invalid Term. No Term exists with id {termId} with Institution Id {institutionId}");
        }

        public async Task<Hold> Update(Hold hold)
        {
            _context.Holds.Update(hold);

            await _context.SaveChangesAsync();

            return await _context.Holds
                .Include(h => h.Term)
                .Include(h => h.Subject)
                .FirstAsync(h => h.Id == hold.Id);
        }
    }
}
