using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class DoctorDetailsDto
    {
        // User fields
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        
        [JsonPropertyName("fullName")]
        public string FullName => $"{FirstName} {LastName}";
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }
        
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        
        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; } // Base64 string for frontend
        
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        // Doctor fields
        [JsonPropertyName("doctorId")]
        public int DoctorId { get; set; }
        
        [JsonPropertyName("bio")]
        public string Bio { get; set; }
        
        [JsonPropertyName("specialization")]
        public string Specialization { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("experienceYears")]
        public int ExperienceYears { get; set; }
        
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        // Optional metadata
        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; }
    }
}
