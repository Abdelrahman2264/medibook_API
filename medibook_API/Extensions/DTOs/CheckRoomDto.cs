using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CheckRoomDto
    {
        [JsonPropertyName("roomName")]
        public string RoomName { get; set; }

        [JsonPropertyName("roomType")]
        public string RoomType { get; set; }

        [JsonPropertyName("roomId")]
        public int? RoomId { get; set; } // Optional: exclude this room ID when checking (for updates)
    }
}
