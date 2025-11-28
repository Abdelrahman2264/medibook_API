using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateAppoinmentDto
    {
      [JsonPropertyName("patientId")]

        public int patient_id { get; set; }
        [JsonPropertyName("doctorId")]

        public int doctor_id { get; set; }
        [JsonPropertyName("appointmentDate")]

        public DateTime appointment_date { get; set; }
    }
}
