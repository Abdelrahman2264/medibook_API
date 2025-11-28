using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CloseAppointmentDto
    {
        [JsonPropertyName("appointmentId")]

        public int appointment_id { get; set; }
        [JsonPropertyName("notes")]

        public string notes { get; set; }
        [JsonPropertyName("medicine")]

        public string medicine { get; set; }
    }
}
