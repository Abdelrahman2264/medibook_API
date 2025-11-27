using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class NurseDetailsDto
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
        public string? ProfileImage { get; set; }

        [JsonPropertyName("dateOfBirth")] // FIXED: Was "dateOfBitrh"
        public DateTime DateOfBirth { get; set; }

        // Nurse fields
        [JsonPropertyName("nurseId")]
        public int NurseId { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("isActive")] // FIXED: Was "isAcrive"
        public bool IsActive { get; set; }

        // Optional metadata
        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; }
    }
}