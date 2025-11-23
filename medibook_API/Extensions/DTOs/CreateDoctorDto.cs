using medibook_API.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Extensions.DTOs
{
    public class CreateDoctorDto
    {
        // User fields
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string mitrial_status { get; set; }

        public byte[]? ProfileImage { get; set; }

        public DateTime DateOfBirth { get; set; }



        public string Bio { get; set; } 
        public string Specialization { get; set; } 
        public string Type { get; set; }

        public int ExperienceYears { get; set; }
       
    }



}
