using System.ComponentModel.DataAnnotations;

namespace medibook_API.Extensions.DTOs
{
    public class UpdateFeedbackDto
    {
        [Required]
        public int FeedbackId { get; set; }

        [StringLength(500, MinimumLength = 1)]
        public string? Comment { get; set; }

    }
}














