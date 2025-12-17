using metiers;
using taxi.DTO.Users;

namespace taxi.Interfaces.Services
{
    public interface IUserAuthService
    {
        Task<User> RegisterAsync(RegisterDTO dto);
        Task<(User user, string token)> LoginAsync(LoginDTO dto);
    }
}
