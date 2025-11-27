using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class UpdateDoctorDto
    {
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }
        
        [JsonPropertyName("specialization")]
        public string? Specialization { get; set; }
        
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        
        [JsonPropertyName("experienceYears")]
        public int? ExperienceYears { get; set; }

        // User fields
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }
        
        [JsonPropertyName("mobilePhone")]
        public string? MobilePhone { get; set; }
        
        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; } // Base64 string from frontend
        [JsonPropertyName("mitrialStatus")]
        public string? MitrialStatus { get; set; }
    }
}
