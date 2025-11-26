using System.ComponentModel.DataAnnotations;

namespace medibook_API.Extensions.DTOs
{
    public class CreateFeedbackDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5")]
        public int Rate { get; set; }
    }
}



