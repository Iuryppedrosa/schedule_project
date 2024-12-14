using scheduler.Models.DTOs;
using scheduler.Models.Entities;
namespace scheduler.Business.Interfaces
{
    public interface IEventBusiness
    {
        Task<EventDTO> GetByGuidAsync(Guid guid);
        Task<IEnumerable<EventDTO>> GetAllAsync();

        Task<EventDTO> CreateAsync(EventDTO NewEvent);
        Task<EventDTO> UpdateAsync(Guid guid, EventDTO NewEvent);
        Task UpdateDeleteAsync(Guid guid);
        Task<IEnumerable<EventDTO>> GetActiveEventsAsync();

    }
}
