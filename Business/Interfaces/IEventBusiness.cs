using scheduler.Models.DTOs;
using scheduler.Models.Entities;
namespace scheduler.Business.Interfaces
{
    public interface IEventBusiness
    {
        Task<Event> GetByIdAsync(int id);
        Task<Event> GetByGuidAsync(Guid guid);
        Task<IEnumerable<EventDTO>> GetAllAsync();

        Task<EventDTO> CreateAsync(EventDTO NewEvent);
        Task<Event> UpdateAsync(Event NewEvent);
        Task UpdateAsync(int id);
        Task<IEnumerable<Event>> GetActiveEventsAsync(Event NewEvent);

    }
}
