using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateFeedbackDto
    {
        [JsonPropertyName("patientId")]

        public int PatientId { get; set; }

        [JsonPropertyName("doctorId")]

        public int DoctorId { get; set; }

        [JsonPropertyName("appointmentId")]
        public int AppointmentId { get; set; }

        [StringLength(500, MinimumLength = 1)]
        [JsonPropertyName("comment")]

        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5")]
        [JsonPropertyName("rate")]

        public int Rate { get; set; }
    }
}








