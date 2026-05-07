using metiers;

namespace taxi.Interfaces.Repositories
{
    public interface IExpenseRepository
    {
        Task AddAsync(Expense expense);
        Task<List<Expense>> GetByTaxiAsync(int taxiId, DateTime start, DateTime end);
        Task<List<Expense>> GetByBossAsync(int bossId);
    }
}
