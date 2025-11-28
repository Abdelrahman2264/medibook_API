using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class AssignAppoinmentDto
    {
        [JsonPropertyName("appointmentId")]

        public int appointment_id { get; set; }
        [JsonPropertyName("nurseId")]

        public int nurse_id { get; set; }
        [JsonPropertyName("roomId")]

        public int room_id { get; set; }

    }
}
