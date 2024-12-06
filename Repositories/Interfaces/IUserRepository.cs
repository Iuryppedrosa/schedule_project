using scheduler.Models.Entities;

namespace scheduler.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task UpdateAsync(int user);
        Task<IEnumerable<User>> GetActiveUsersAsync();
    }
}
