using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class FeedbackResponseDto
    {
        [JsonPropertyName("feedbackId")]
        public int FeedbackId { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("success")]

        public bool Success { get; set; }
    }
}









