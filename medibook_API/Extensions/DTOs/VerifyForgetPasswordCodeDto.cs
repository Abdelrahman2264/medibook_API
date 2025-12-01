using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class VerifyForgetPasswordCodeDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}











