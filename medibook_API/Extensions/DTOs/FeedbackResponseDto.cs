namespace medibook_API.Extensions.DTOs
{
    public class FeedbackResponseDto
    {
        public int FeedbackId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}

