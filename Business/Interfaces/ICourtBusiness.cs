using scheduler.Models.Entities;

namespace scheduler.Business.Interfaces;
public interface ICourtBusiness
{
   Task<Court> GetByIdAsync(int id);
    Task<Court> GetByGuidAsync(Guid? guid);
    Task<IEnumerable<Court>> GetAllAsync();

    Task<Court> CreateAsync(Court NewCourt);
    Task<Court> UpdateAsync(Court NewCourt);
    Task UpdateAsync(int id);
    Task<IEnumerable<Court>> GetActiveCourtsAsync(Court NewCourt);
}

