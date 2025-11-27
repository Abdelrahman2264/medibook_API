using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class UpdateNurseDto
    {
        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("mobilePhone")]
        public string? MobilePhone { get; set; }

        [JsonPropertyName("profileImage")]
        public string? ProfileImage { get; set; }

        [JsonPropertyName("mitrialStatus")] // ADD THIS
        public string? MitrialStatus { get; set; }
    }
}