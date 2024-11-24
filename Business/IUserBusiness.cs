using scheduler.Models.DTOs;
using scheduler.Models.Entities;

namespace scheduler.Business
{
    public interface IUserBusiness
    {
        Task<UserDTO> GetByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserCreateDTO> CreateAsync(UserCreateDTO user);
        Task UpdateAsync(UserCreateDTO user, int id);
        Task DeleteAsync(int id);
        Task<LoginResponse> AuthenticateAsync(string email, string password);
    }
}
