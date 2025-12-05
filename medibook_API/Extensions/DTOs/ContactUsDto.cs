using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class ContactUsDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 200 characters")]
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters")]
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}

