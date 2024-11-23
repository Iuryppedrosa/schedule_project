using scheduler.Models.DTOs;
using scheduler.Models.Entities;

namespace scheduler.Business
{
    public interface IUserBusiness
    {
        Task<UserDTO> GetByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<LoginResponse> AuthenticateAsync(string email, string password);
    }
}
