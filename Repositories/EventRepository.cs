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

        public async Task<Event> CreateAsync(Event NewEvent)
        {
            NewEvent.CreatedDate = DateTime.UtcNow;
            NewEvent.Guid = Guid.NewGuid();

            await _context.Events.AddAsync(NewEvent);
            await _context.SaveChangesAsync();
            return NewEvent;
        }

        public async Task UpdateAsync(Event NewEvent)
        {
            NewEvent.UpdatedDate = DateTime.UtcNow;
            _context.Events.Update(NewEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id)
        {
            var NewEvent = await _context.Events.FindAsync(id);
            if(NewEvent == null)
                throw new Exception("Event not found");

            NewEvent.DeletedDate = DateTime.UtcNow;
            _context.Events.Update(NewEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(Event NewEvent)
        {
            return await _context.Events.Where(e => e.DeletedDate == null).ToListAsync();
        }


    }
}
