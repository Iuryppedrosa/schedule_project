using Microsoft.IdentityModel.Tokens;
using scheduler.Models.DTOs;
using scheduler.Models.Entities;
using scheduler.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace scheduler.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;

        public UserBusiness(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Guid = user.Guid,
                FederalId = user.FederalId,
                Name = user.Name,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate,
                DeletedDate = user.DeletedDate
            };

            return userDTO;
            
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            if (users == null)
                throw new Exception("No users found");

            var userDTOs = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Guid = u.Guid,
                FederalId = u.FederalId,
                Name = u.Name,
                Email = u.Email,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate,
                DeletedDate = u.DeletedDate
            }).ToList();
            return userDTOs;
        }

        public async Task<User> CreateAsync(User user)
        {
            var existingUser = await _repository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var password = "standardPassword";
            user.PasswordHash = HashPassord(password);
            return await _repository.CreateAsync(user);
        }

        private string HashPassord(string password)
        {
            // Implement password hashing
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _repository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            await _repository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            await _repository.DeleteAsync(id);
        }

        public async Task<LoginResponse> AuthenticateAsync(string email, string password)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = GenerateJwtToken(user);

            return new LoginResponse(token, user);
        }

        private bool VerifyPassword(string password, string hash)
        {
            // Implement password verification
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private string GenerateJwtToken(User user)
        {
            // Implement JWT token generation
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
