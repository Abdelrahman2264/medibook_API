namespace medibook_API.Extensions.DTOs
{
    public class FeedbackDetailsDto
    {
        public int FeedbackId { get; set; }
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorFirstName { get; set; } = string.Empty;
        public string DoctorLastName { get; set; } = string.Empty;
        public string DoctorSpecialization { get; set; } = string.Empty;
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
        public DateTime FeedbackDate { get; set; }
        public string DoctorReply { get; set; } = string.Empty;
        public DateTime? ReplyDate { get; set; }
        public bool IsFavourite { get; set; }
    }
}





