using Microsoft.EntityFrameworkCore;
using scheduler.Data;
using scheduler.Models.Entities;
using scheduler.Repositories.Interfaces;

namespace scheduler.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetByGuidAsync(Guid? guid)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Guid == guid && u.DeletedDate == null);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.DeletedDate == null);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Where(u => u.DeletedDate == null).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync(IEnumerable<Guid?> guids)
        {
            return await _context.Users.Where(u => guids.Contains(u.Guid)).ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            user.CreatedDate = DateTime.UtcNow;
            user.Guid = Guid.NewGuid();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            user.UpdatedDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
                throw new Exception("User not found");

            user.DeletedDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(user => user.DeletedDate == null)
                .ToListAsync();
        }
    }
}
