using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateDoctorDto
    {
        // User fields
        [Required(ErrorMessage = "First name is required")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name is required")]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mobile phone is required")]
        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Gender is required")]
        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Marital status is required")]
        [JsonPropertyName("mitrialStatus")]
        public string MitrialStatus { get; set; } = string.Empty;

        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; } // Base64 string from frontend

        [Required(ErrorMessage = "Date of birth is required")]
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Specialization is required")]
        [JsonPropertyName("specialization")]
        public string Specialization { get; set; } = string.Empty;
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Experience years is required")]
        [Range(0, 100, ErrorMessage = "Experience years must be between 0 and 100")]
        [JsonPropertyName("experienceYears")]
        public int ExperienceYears { get; set; }
    }
}
