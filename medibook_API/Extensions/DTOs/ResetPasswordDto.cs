using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class ResetPasswordDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
        
        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}











