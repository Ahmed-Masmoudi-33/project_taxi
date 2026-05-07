using metiers;
using taxi.Data;
using taxi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace taxi.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationContext _context;

        public ExpenseRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Expense>> GetByTaxiAsync(int taxiId, DateTime start, DateTime end)
        {
            return await _context.Expenses
                .Where(e => e.TaxiId == taxiId &&
                            e.ExpenseDate >= start &&
                            e.ExpenseDate <= end)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetByBossAsync(int bossId)
        {
            // Get all taxis owned by the boss, then get all expenses for those taxis
            var bossTaxiIds = await _context.Taxis
                .Where(t => t.BossId == bossId)
                .Select(t => t.Id)
                .ToListAsync();

            if (!bossTaxiIds.Any())
            {
                return new List<Expense>();
            }

            return await _context.Expenses
                .Where(e => bossTaxiIds.Contains(e.TaxiId))
                .Include(e => e.Taxi)
                .OrderByDescending(e => e.ExpenseDate)
                .ThenBy(e => e.PlateNumber)
                .ToListAsync();
        }
    }
}
