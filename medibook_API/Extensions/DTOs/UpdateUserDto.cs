using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class UpdateUserDto
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }
        [JsonPropertyName("lastName")]

        public string? LastName { get; set; }
        [JsonPropertyName("mobilePhone")]

        public string? MobilePhone { get; set; }
        [JsonPropertyName("gender")]

        public string? Gender { get; set; }
        [JsonPropertyName("mitrialStatus")]

        public string? MitrialStatus { get; set; }
        [JsonPropertyName("profileImage")]

        public string? ProfileImage { get; set; }

    }
}
