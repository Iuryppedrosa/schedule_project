using scheduler.Models.Entities;
namespace scheduler.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> GetByIdAsync(int id);
        Task<Event> GetByGuidAsync(Guid guid);
        Task<IEnumerable<Event>> GetAllAsync();

        Task<Event> CreateAsync(Event NewEvent);

        Task<Event> UpdateAsync(Event NewEvent);

        Task UpdateAsync(int id);
        Task<IEnumerable<Event>> GetActiveEventsAsync(Event NewEvent);

    }
}
