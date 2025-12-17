using metiers;

namespace taxi.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByPhoneAsync(string phoneNumber);

        Task<List<User>> GetAllEmployeesAsync();
        Task<List<User>> GetAllBossesAsync();

        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
