namespace medibook_API.Extensions.DTOs
{
    using System.Text.Json.Serialization;

    namespace medibook_API.Extensions.DTOs
    {
        public class FeedbackDetailsDto
        {
            [JsonPropertyName("feedbackId")]
            public int FeedbackId { get; set; }

            [JsonPropertyName("patientId")]
            public int PatientId { get; set; }

            [JsonPropertyName("patientFirstName")]
            public string PatientFirstName { get; set; } = string.Empty;

            [JsonPropertyName("patientLastName")]
            public string PatientLastName { get; set; } = string.Empty;

            [JsonPropertyName("patientEmail")]
            public string PatientEmail { get; set; } = string.Empty;

            [JsonPropertyName("doctorId")]
            public int DoctorId { get; set; }

            [JsonPropertyName("doctorFirstName")]
            public string DoctorFirstName { get; set; } = string.Empty;

            [JsonPropertyName("doctorLastName")]
            public string DoctorLastName { get; set; } = string.Empty;

            [JsonPropertyName("doctorSpecialization")]
            public string DoctorSpecialization { get; set; } = string.Empty;

            [JsonPropertyName("appointmentId")]
            public int AppointmentId { get; set; }

            [JsonPropertyName("appointmentDate")]
            public DateTime AppointmentDate { get; set; }

            [JsonPropertyName("comment")]
            public string Comment { get; set; } = string.Empty;

            [JsonPropertyName("rate")]
            public int Rate { get; set; }

            [JsonPropertyName("feedbackDate")]
            public DateTime FeedbackDate { get; set; }

            [JsonPropertyName("doctorReply")]
            public string DoctorReply { get; set; } = string.Empty;

            [JsonPropertyName("replyDate")]
            public DateTime? ReplyDate { get; set; }

            [JsonPropertyName("isFavourite")]
            public bool IsFavourite { get; set; }
        }
    }
}














