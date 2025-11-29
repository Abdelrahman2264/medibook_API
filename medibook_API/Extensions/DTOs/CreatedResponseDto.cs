using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreatedResponseDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("typeId")]

        public int TypeId { get; set; }
        [JsonPropertyName("message")]

        public string Message { get; set; }
    }
}
