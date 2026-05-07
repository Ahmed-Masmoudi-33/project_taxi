using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taxi.Data;
using taxi.DTO.Users;
using taxi.Interfaces.Services;
using metiers;
namespace taxi.Controllers
{
    [ApiController]
    [Route("taxi/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserAuthService _authService;

        public UserController(IUserAuthService authService)
        {
            _authService = authService;
        }

        // POST: taxi/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var user = await _authService.RegisterAsync(dto);

                return Ok(new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                // Include inner exception details for database errors
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                }
                return BadRequest(new { message = errorMessage });
            }
        } 

        // POST: taxi/users/login
      [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var (user, token) = await _authService.LoginAsync(dto);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.PhoneNumber,
                        user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                // Include inner exception details for database errors
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                }
                return BadRequest(new { message = errorMessage });
            }
        }

    }
}
