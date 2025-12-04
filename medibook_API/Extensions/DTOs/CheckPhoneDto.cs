using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CheckPhoneDto
    {
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("userId")]
        public int? UserId { get; set; } // Optional: exclude this user ID when checking (for updates)
    }
}
