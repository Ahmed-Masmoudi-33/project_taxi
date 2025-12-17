using Enums;
using metiers;
using taxi.DTO.Users;
using taxi.Interfaces.Repositories;
using taxi.Interfaces.Services;

namespace taxi.Services
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public UserAuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<User> RegisterAsync(RegisterDTO dto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new Exception("FirstName is required");
            
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new Exception("LastName is required");
            
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new Exception("PhoneNumber is required");
            
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new Exception("Password is required");
            
            if (dto.Password.Length > 8)
                throw new Exception("Password must be 8 characters or less");

            // Check existing user
            var existingUser = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);
            if (existingUser != null)
                throw new Exception("User already exists");

            // Validate role
            if (dto.Role != UserRoles.BOSS && dto.Role != UserRoles.EMPLOYEE)
                throw new Exception("Invalid role");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                Password = dto.Password,
                CIN = dto.CIN
            };

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<(User user, string token)> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);
            if (user == null)
                throw new Exception("Invalid phone number");

            if (string.IsNullOrWhiteSpace(dto.Password) || user.Password != dto.Password)
                throw new Exception("Invalid password");

            var token = _jwtTokenService.GenerateToken(user);
            return (user, token);
        }
    }
}
