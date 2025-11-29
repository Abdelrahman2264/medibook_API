using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class NotificationDetailsDto
    {
        [JsonPropertyName("notificationId")]
        public int NotificationId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("readDate")]
        public DateTime? ReadDate { get; set; }

        [JsonPropertyName("senderUserId")]
        public int SenderUserId { get; set; }

        [JsonPropertyName("senderName")]
        public string SenderName { get; set; } = string.Empty;

        [JsonPropertyName("senderEmail")]
        public string SenderEmail { get; set; } = string.Empty;

        [JsonPropertyName("receiverUserId")]
        public int ReceiverUserId { get; set; }

        [JsonPropertyName("receiverName")]
        public string ReceiverName { get; set; } = string.Empty;

        [JsonPropertyName("receiverEmail")]
        public string ReceiverEmail { get; set; } = string.Empty;
    }
}