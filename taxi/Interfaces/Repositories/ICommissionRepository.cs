using metiers;

namespace taxi.Interfaces.Repositories
{
    public interface ICommissionRepository
    {
        Task<Commission?> GetByBossAsync(int bossId);
        Task AddAsync(Commission commission);
        Task UpdateAsync(Commission commission);
    }
}
