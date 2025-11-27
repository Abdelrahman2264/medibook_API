using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CheckEmailDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}



