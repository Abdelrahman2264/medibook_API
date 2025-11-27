using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateRoomDto
    {
        [JsonPropertyName("roomName")]

        public string RoomName { get; set; }
        [JsonPropertyName("roomType")]

        public string RoomType { get; set; }
    }
}
