using medibook_API.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateNurseDto
    {
        // User fields
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("mitrialStatus")]
        public string MitrialStatus { get; set; }
        [JsonPropertyName("profileImage")]

        public byte[]? ProfileImage { get; set; }
        [JsonPropertyName("dateOfBirth")]

        public DateTime DateOfBirth { get; set; }


        [JsonPropertyName("bio")]

        public string Bio { get; set; }

    }
}
