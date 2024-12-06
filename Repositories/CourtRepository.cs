using scheduler.Data;
using scheduler.Repositories.Interfaces;
using scheduler.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace scheduler.Repositories
{
    public class CourtRepository : ICourtRepository
    {
        private readonly ApplicationDbContext _context;
        public CourtRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Court> GetByIdAsync(int id)
        {
            return await _context.Courts.FindAsync(id);
        }

        public async Task<Court> GetByGuidAsync(Guid? guid)
        {
            return await _context.Courts.FirstOrDefaultAsync(c => c.Guid == guid && c.DeletedDate == null);
        }

        public async Task<IEnumerable<Court>> GetAllAsync()
        {
            return await _context.Courts.Where(c => c.DeletedDate == null).ToListAsync();
        }

        public async Task<Court> CreateAsync(Court court)
        {
            court.CreatedDate = DateTime.UtcNow;
            court.Guid = Guid.NewGuid();

            await _context.Courts.AddAsync(court);
            await _context.SaveChangesAsync();
            return court;
        }

        public async Task<Court> UpdateAsync(Court court)
        {
            court.UpdatedDate = DateTime.UtcNow;
            _context.Courts.Update(court);
            await _context.SaveChangesAsync();
            return court;
        }

        public async Task UpdateAsync(int id)
        {
            var court = await _context.Courts.FindAsync(id);
            if(court == null)
                throw new Exception("Court not found");

            court.DeletedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Court>> GetActiveCourtsAsync(Court court)
        {
            return await _context.Courts.Where(c => c.DeletedDate == null && c.Active).ToListAsync();
        }


        
        
    }
}
