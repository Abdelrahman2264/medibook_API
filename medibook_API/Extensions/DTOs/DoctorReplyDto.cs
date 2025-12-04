using System.ComponentModel.DataAnnotations;

namespace medibook_API.Extensions.DTOs
{
    public class DoctorReplyDto
    {
        [Required]
        public int FeedbackId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string DoctorReply { get; set; } = string.Empty;
    }
}














