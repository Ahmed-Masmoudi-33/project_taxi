using System.Text.Json.Serialization;

namespace taxi.DTO.Users
{
    public class LoginDTO
    {
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
