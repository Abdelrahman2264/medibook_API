using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CancelAppointmentDto
    {
        [JsonPropertyName("appointmentId")]

        public int appointment_id { get; set; }
        [JsonPropertyName("cancellationReason")]

        public string cancellation_reason { get; set; }
        
    }
}
