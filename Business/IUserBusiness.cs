using scheduler.Models.Entities;

namespace scheduler.Business
{
    public interface IUserBusiness
    {
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<string> AuthenticateAsync(string email, string password);
    }
}
