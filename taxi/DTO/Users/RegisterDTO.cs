using Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace taxi.DTO.Users
{
    public class RegisterDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
/*        [Required]
        public string Email { get; set; }*/
        [Required]
        public string CIN { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = UserRoles.EMPLOYEE;
    }
}
