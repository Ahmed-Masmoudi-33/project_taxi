using Enums;
using metiers;
using taxi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using taxi.Data;

namespace taxi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.OwnedTaxis)
                .Include(u => u.Assignments)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetByCINAsync(string cin)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.CIN == cin);
        }

        public async Task<List<User>> GetAllEmployeesAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRoles.EMPLOYEE)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllBossesAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRoles.BOSS)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                throw new Exception($"Database error: {ex.Message}. Inner: {ex.InnerException?.Message}", ex);
            }
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
