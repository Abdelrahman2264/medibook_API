using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class UpdateRoomDto
    {
        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }
        [JsonPropertyName("roomName")]

        public string RoomName { get; set; }
        [JsonPropertyName("roomType")]

        public string RoomType { get; set; }
    }
}
