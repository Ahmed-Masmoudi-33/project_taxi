using metiers;
using taxi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using taxi.Data;    

namespace taxi.Repositories
{
    public class CommissionRepository : ICommissionRepository
    {
        private readonly ApplicationContext _context;

        public CommissionRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Commission?> GetByBossAsync(int bossId)
        {
            return await _context.Commissions
                .FirstOrDefaultAsync(c => c.BossId == bossId);
        }

        public async Task AddAsync(Commission commission)
        {
            _context.Commissions.Add(commission);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Commission commission)
        {
            _context.Commissions.Update(commission);
            await _context.SaveChangesAsync();
        }
    }
}