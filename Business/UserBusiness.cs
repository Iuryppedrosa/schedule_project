using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using scheduler.Business.Interfaces;
using scheduler.Configuration;
using scheduler.Models.DTOs;
using scheduler.Models.Entities;
using scheduler.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace scheduler.Business
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _repository;
        private readonly JwtConfig _jwtConfig; // Injete a classe de configuração aqui


        public UserBusiness(IUserRepository repository, IOptions<JwtConfig> jwtConfigOptions)
        {
            _repository = repository;
            _jwtConfig = jwtConfigOptions.Value;
        }

        public async Task<UserDTO> GetByGuidAsync(Guid? guid)
        {
            var user = await _repository.GetByGuidAsync(guid);
            if (user == null)
                throw new Exception("User not found");

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Guid = user.Guid,
                FederalId = user.FederalId,
                Name = user.Name,
                Email = user.Email,
                Contact = user.Contact,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate,
                DeletedDate = user.DeletedDate
            };

            return userDTO;
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
                Contact = user.Contact,
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
                Contact = u.Contact,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate,
                DeletedDate = u.DeletedDate
            }).ToList();
            return userDTOs;
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsyncByGuid(IEnumerable<Guid?> guids)
        {
            var users = await _repository.GetAllAsync(guids);
            if (!users.Any())
                throw new Exception("No users found");

            var userDTOs = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Guid = u.Guid,
                FederalId = u.FederalId,
                Name = u.Name,
                Email = u.Email,
                Contact = u.Contact,
                CreatedDate = u.CreatedDate,
                UpdatedDate = u.UpdatedDate,
                DeletedDate = u.DeletedDate
            }).ToList();

            return userDTOs;
        }

        public async Task<UserCreateDTO> CreateAsync(UserCreateDTO userCreateDTO)
        {
            // Validação e criação do domínio
            var user = new User
            {
                FederalId = userCreateDTO.FederalId,
                Name = userCreateDTO.Name,
                Email = userCreateDTO.Email,
                Contact = userCreateDTO.Contact,
                PasswordHash = HashPassord("standardPassword")
            };

            var createdUser = await _repository.CreateAsync(user);

            // Transformação para DTO
            return new UserCreateDTO
            {
                FederalId = createdUser.FederalId,
                Name = createdUser.Name,
                Email = createdUser.Email,
                Contact = createdUser.Contact

            };
        }

        private string HashPassord(string password)
        {
            // Implement password hashing
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task UpdateAsync(UserCreateDTO newUser, int id)
        { 
            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null)
                throw new Exception("User not found");

            existingUser.FederalId = newUser.FederalId;
            existingUser.Name = newUser.Name;
            existingUser.Email = newUser.Email;
            existingUser.Contact = newUser.Contact;
            existingUser.PasswordHash = existingUser.PasswordHash;

            await _repository.UpdateAsync(existingUser);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.UpdateAsync(id);
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Adiciona as "claims" (informações do usuário) ao token.
                // Isso é essencial para a autorização nos endpoints protegidos.
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()), // Claim para o ID do usuário
                    new Claim(ClaimTypes.Email, user.Email)         // Claim para o email
                    // Você pode adicionar outras claims se necessário
                }),

                // Adiciona a data de expiração do token.
                Expires = DateTime.UtcNow.AddHours(_jwtConfig.ExpiresInHours),

                // Mantém as configurações que você já tinha.
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
