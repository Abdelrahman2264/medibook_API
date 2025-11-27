using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class UserDetailsDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("firstName")]

        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]

        public string LastName { get; set; }
        [JsonPropertyName("email")]

        public string Email { get; set; }
        [JsonPropertyName("mobilePhone")]

        public string MobilePhone { get; set; }
        [JsonPropertyName("gender")]

        public string Gender { get; set; }
        [JsonPropertyName("MitrialStatus")]

        public string MitrialStatus { get; set; }
        [JsonPropertyName("profileImage")]

        public string? ProfileImage { get; set; }
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
