using scheduler.Models.Entities;
using scheduler.Business.Interfaces;
using scheduler.Repositories.Interfaces;
using scheduler.Business.Interfaces;

namespace scheduler.Business
{
    public class CourtBusiness : ICourtBusiness
    {
        private readonly ICourtRepository _courtRepository;

        public CourtBusiness(ICourtRepository repository)
        {
            _courtRepository = repository;
        }

        public async Task<Court> GetByIdAsync(int id)
        {
            var court = await _courtRepository.GetByIdAsync(id);
            if (court == null)
                throw new Exception("Court not found");

            return court;
        }

        public async Task<Court> GetByGuidAsync(Guid? guid)
        {
            var court = await _courtRepository.GetByGuidAsync(guid);
            if (court == null)
                throw new Exception("Court not found");

            return court;
        }

        public async Task<IEnumerable<Court>> GetAllAsync()
        {
            var courts = await _courtRepository.GetAllAsync();
            if (courts == null)
                throw new Exception("No Courts found");

            return courts;
        }

        public async Task<Court> CreateAsync(Court createCourt)
        {
            var newCourt = new Court
            {
                Name = createCourt.Name,
                Place = createCourt.Place,
                Active = true,
               
            };

            return await _courtRepository.CreateAsync(newCourt);
        }

        public async Task<Court> UpdateAsync(Court updateCourt)
        {
            var court = await _courtRepository.GetByIdAsync(updateCourt.Id);
            if (court == null)
                throw new Exception("Court not found");

            court.Name = updateCourt.Name;
            court.Place = updateCourt.Place;
            court.Active = updateCourt.Active;
            court.UpdatedDate = DateTime.UtcNow;

            return await _courtRepository.UpdateAsync(court);
        }

        public async Task UpdateAsync(int id)
        {
            var court = await _courtRepository.GetByIdAsync(id);
            if (court == null)
                throw new Exception("Court not found");

            court.DeletedDate = DateTime.UtcNow;
            await _courtRepository.UpdateAsync(id);
        }

        public async Task<IEnumerable<Court>> GetActiveCourtsAsync(Court court)
        {
            var courts = await _courtRepository.GetActiveCourtsAsync(court);
            if (courts == null)
                throw new Exception("No Courts found");

            return courts;
        }
    }
}
