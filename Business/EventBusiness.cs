using scheduler.Business.Interfaces;
using scheduler.Models.Entities;
using scheduler.Repositories.Interfaces;

namespace scheduler.Business
{
    public class EventBusiness : IEventBusiness
    {
        private readonly IEventRepository _repository;

        public EventBusiness(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            var Event = await _repository.GetByIdAsync(id);
            if (Event == null)
                throw new Exception("Event not found");

            return Event;
        }

        public async Task<Event> GetByGuidAsync(Guid guid)
        {
            var Event = await _repository.GetByGuidAsync(guid);
            if (Event == null)
                throw new Exception("Event not found");

            return Event;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            var Events = await _repository.GetAllAsync();
            if (Events == null)
                throw new Exception("No Events found");

            return Events;
        }

        public async Task<Event> CreateAsync(Event NewEvent)
        {
            var Event = await _repository.CreateAsync(NewEvent);
            if (Event == null)
                throw new Exception("Event not created");

            return Event;
        }

        public async Task<Event> UpdateAsync(Event NewEvent)
        {
            var Event = await _repository.UpdateAsync(NewEvent);
            if (Event == null)
                throw new Exception("Event not updated");

            return Event;
        }

        public async Task UpdateAsync(int id)
        {
            var Event = await _repository.GetByIdAsync(id);
            if (Event == null)
                throw new Exception("Event not found");

            await _repository.UpdateAsync(Event);
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(Event NewEvent)
        {
            var Events = await _repository.GetActiveEventsAsync(NewEvent);
            if (Events == null)
                throw new Exception("No Events found");

            return Events;
        }

    }
}
