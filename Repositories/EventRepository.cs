using scheduler.Models.Entities;
using scheduler.Repositories.Interfaces;
using scheduler.Data;
using Microsoft.EntityFrameworkCore;
namespace scheduler.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<Event> GetByGuidAsync(Guid guid)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Guid == guid);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> CreateAsync(Event newEvent)
        {
            newEvent.Guid = Guid.NewGuid();
            newEvent.CreatedDate = DateTime.UtcNow;

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        public async Task UpdateDeleteAsync(Event deleteEvent)
        {
            deleteEvent.UpdatedDate = DateTime.UtcNow;
            deleteEvent.DeletedDate = DateTime.UtcNow;
            _context.Events.Update(deleteEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<Event> UpdateAsync(Event NewEvent)
        {
            NewEvent.UpdatedDate = DateTime.UtcNow;
            _context.Events.Update(NewEvent);
            await _context.SaveChangesAsync();
            return NewEvent;
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync()
        {
            return await _context.Events.Where(e => e.DeletedDate == null).ToListAsync();
        }

    }
}
