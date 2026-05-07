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
    }
}
