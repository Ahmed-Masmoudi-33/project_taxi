using metiers;
using Microsoft.EntityFrameworkCore;
using taxi.Data;
using taxi.Interfaces.Repositories;

namespace taxi.Repositories
{
    public class TaxiRepository : ITaxiRepository
    {
        private readonly ApplicationContext _context;


        public TaxiRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Taxi?> GetByIdAsync(int id)
        {
            return await _context.Taxis
                .Include(t => t.Rides)
                .Include(t => t.Expenses)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.Employee)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Taxi>> GetAllByBossAsync(int bossId)
        {
            return await _context.Taxis
                .Where(t => t.BossId == bossId)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.Employee)
                .ToListAsync();
        }

        public async Task AddAsync(Taxi taxi)
        {
            _context.Taxis.Add(taxi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Taxi taxi)
        {
            _context.Taxis.Update(taxi);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var taxi = await _context.Taxis
                .Include(t => t.Expenses)
                .Include(t => t.Assignments)
                .Include(t => t.Rides)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taxi == null)
                return false;

            _context.Expenses.RemoveRange(taxi.Expenses);
            _context.Assignments.RemoveRange(taxi.Assignments);
            _context.Rides.RemoveRange(taxi.Rides);
            _context.Taxis.Remove(taxi);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
