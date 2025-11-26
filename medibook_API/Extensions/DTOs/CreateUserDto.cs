using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateUserDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }
        
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        
        [JsonPropertyName("mitrialStatus")]
        public string MitrialStatus { get; set; }
        
        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; } // Base64 string from frontend
        
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
    }
}
