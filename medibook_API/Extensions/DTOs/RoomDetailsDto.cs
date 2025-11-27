using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class RoomDetailsDto
    {

        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }
        [JsonPropertyName("roomName")]

        public string RoomName { get; set; }
        [JsonPropertyName("roomType")]

        public string RoomType { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; }



    }

}
