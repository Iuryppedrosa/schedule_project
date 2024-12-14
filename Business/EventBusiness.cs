using Microsoft.Extensions.Logging;
using scheduler.Business.Interfaces;
using scheduler.Models.DTOs;
using scheduler.Models.Entities;
using scheduler.Repositories.Interfaces;

namespace scheduler.Business
{
    public class EventBusiness : IEventBusiness
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserBusiness _userBusiness;
        private readonly ICourtBusiness _courtBusiness;

        public EventBusiness(IEventRepository repository, IUserBusiness userBusiness, ICourtBusiness courtBusiness)
        {
            _eventRepository = repository;
            _userBusiness = userBusiness;
            _courtBusiness = courtBusiness;
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            var Event = await _eventRepository.GetByIdAsync(id);
            if (Event == null)
                throw new Exception("Event not found");

            return Event;
        }

        public async Task<EventDTO> GetByGuidAsync(Guid guid)
        {
            var Event = await _eventRepository.GetByGuidAsync(guid);
            if (Event == null)
                throw new Exception("Event not found");

            var user = await _userBusiness.GetByGuidAsync(Event.UserGuid);
            if(user == null)
                throw new Exception("User not found");

            var court = await _courtBusiness.GetByGuidAsync(Event.CourtGuid);
            if(court == null)
                throw new Exception("Court not found");

            var EventDTO = new EventDTO
                {
                Id = Event.Id,
                Guid = Event.Guid,
                UserGuid = Event.UserGuid,
                UserName = user.Name,
                CourtGuid = Event.CourtGuid,
                Title = Event.Title,
                Details = Event.Details,
                StartDate = Event.StartDate,
                EndDate = Event.EndDate
            };

            return EventDTO;
        }

        public async Task<IEnumerable<EventDTO>> GetAllAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            if (!events.Any())
                throw new Exception("No Events found");

            var userGuids = events.Select(e => e.UserGuid).Distinct().ToList();
            var users = await _userBusiness.GetAllAsyncByGuid(userGuids);

            var eventDTOs = events
                .Join(users, 
                    evt => evt.UserGuid, 
                    usr => usr.Guid, 
                    (evt, usr) => new EventDTO 
                    {
                        Id = evt.Id,
                        Guid = evt.Guid,
                        UserGuid = evt.UserGuid,
                        UserName = usr.Name, 
                        CourtGuid = evt.CourtGuid,
                        Title = evt.Title,
                        Details = evt.Details,
                        StartDate = evt.StartDate,
                        EndDate = evt.EndDate
                    })
                .ToList(); 

            return eventDTOs;
        }

        public async Task<EventDTO> CreateAsync(EventDTO createEvent)
        {
               
            var user = await _userBusiness.GetByGuidAsync(createEvent.UserGuid);
            if(user == null)
                throw new Exception("User not found");

            var court = await _courtBusiness.GetByGuidAsync(createEvent.CourtGuid);
            if(court == null)
                throw new Exception("Court not found");

            var newEvent = new Event
            {
                Title = createEvent.Title,
                UserId = user.Id,
                UserFederalId = user.FederalId,
                UserGuid = user.Guid,
                Details = createEvent.Details,
                CourtId = court.Id,
                CourtGuid = court.Guid,
                CourtName = court.Name,
                StartDate = createEvent.StartDate,
                EndDate = createEvent.EndDate,
            };
           

            var createdEvent = await _eventRepository.CreateAsync(newEvent);
            return new EventDTO
            {
                Id = createdEvent.Id,
                Guid = createdEvent.Guid,
                UserGuid = createdEvent?.UserGuid,
                UserName = user?.Name,
                CourtGuid = createdEvent?.CourtGuid,
                Title = createdEvent?.Title,
                Details = createdEvent?.Details,
                StartDate = createdEvent.StartDate,
                EndDate = createdEvent.EndDate
            };
        }

        public async Task<EventDTO> UpdateAsync(Guid oldEventGuid, EventDTO newEvent)
        {
            var oldEvent = await _eventRepository.GetByGuidAsync(oldEventGuid);
            if (oldEvent == null)
                throw new Exception("Event not found");

            var user = await _userBusiness.GetByGuidAsync(newEvent.UserGuid);
            if(user == null)
                throw new Exception("User not found");

            var court = await _courtBusiness.GetByGuidAsync(newEvent.CourtGuid);
            if(court == null)
                throw new Exception("Court not found");



            oldEvent.Title = newEvent.Title;
            oldEvent.UserId = user.Id;
            oldEvent.UserFederalId = user.FederalId;
            oldEvent.UserGuid = user.Guid;
            oldEvent.Details = newEvent.Details;
            oldEvent.CourtId = court.Id;
            oldEvent.CourtGuid = court.Guid;
            oldEvent.CourtName = court.Name;
            oldEvent.StartDate = newEvent.StartDate;
            oldEvent.EndDate = newEvent.EndDate;

           

            var eventNew = await _eventRepository.UpdateAsync(oldEvent);

            var response = new EventDTO
            {
                Id = eventNew.Id,
                Guid = eventNew.Guid,
                UserGuid = eventNew.UserGuid,
                UserName = user.Name,
                CourtGuid = eventNew.CourtGuid,
                Title = eventNew.Title,
                Details = eventNew.Details,
                StartDate = eventNew.StartDate,
                EndDate = eventNew.EndDate
            };


            return response;
        }

        public async Task UpdateDeleteAsync(Guid guid)
        {
            var deleteEvent = await _eventRepository.GetByGuidAsync(guid);
            if (deleteEvent == null)
                throw new Exception("Event not found");

            await _eventRepository.UpdateDeleteAsync(deleteEvent);
        }

        public async Task<IEnumerable<EventDTO>> GetActiveEventsAsync()
        {
            var events = await _eventRepository.GetActiveEventsAsync();
            if (events == null)
                throw new Exception("No Events found");

            var userGuids = events.Select(e => e.UserGuid).Distinct().ToList();
            var users = await _userBusiness.GetAllAsyncByGuid(userGuids);

            var eventDTOs = events
                .Join(users,
                    evt => evt.UserGuid,
                    usr => usr.Guid,
                    (evt, usr) => new EventDTO
                    {
                        Id = evt.Id,
                        Guid = evt.Guid,
                        UserGuid = evt.UserGuid,
                        UserName = usr.Name,
                        CourtGuid = evt.CourtGuid,
                        Title = evt.Title,
                        Details = evt.Details,
                        StartDate = evt.StartDate,
                        EndDate = evt.EndDate
                    })
                .ToList();

            return eventDTOs;
        }

    }
}
