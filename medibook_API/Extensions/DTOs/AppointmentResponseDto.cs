using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class AppointmentResponseDto
    {
        [JsonPropertyName("appointmentId")]

        public int appointment_id { get; set; }
        [JsonPropertyName("appointmentDate")]

        public DateTime appointment_date { get; set; }
        [JsonPropertyName("appointmentTime")]


        public string appointment_time { get; set; }
        [JsonPropertyName("message")]

        public string message { get; set; }

    }
}
