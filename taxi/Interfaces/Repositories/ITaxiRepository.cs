using metiers;

namespace taxi.Interfaces.Repositories
{
    public interface ITaxiRepository
    {
        Task<Taxi?> GetByIdAsync(int id);
        Task<List<Taxi>> GetAllByBossAsync(int bossId);

        Task AddAsync(Taxi taxi);
        Task UpdateAsync(Taxi taxi);
    }
}
