using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Users
    {
        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("user_id")]
        public int user_id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string mitrial_status { get; set; } = string.Empty;
        public bool is_active { get; set; } = true;
        public string email { get;set; } = string.Empty;
        public string mobile_phone { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        [ForeignKey("Role")]
        [Column(TypeName = "INT")]
        [Required]
        public int role_id { get; set; }
        public Roles Role { get;set; } 

        [Column(TypeName = "VARBINARY(MAX)")]
        [MaxLength(5242880, ErrorMessage = "Image size cannot exceed 5MB")]
        public byte[]? profile_image { get; set; }
        public DateTime create_date { get; set; } = DateTime.Now;
        public DateTime date_of_birth { get; set; }


        public ICollection<Nurses> Nurses { get; set; }
        public ICollection<Doctors> Doctors { get; set; }
        public ICollection<Logs> Logs { get; set; }
        public ICollection<Notifications> Notifications { get; set; }   
        public ICollection<Appointments> Appointments { get; set; }   
        public ICollection<FeedBacks> FeedBacks { get; set; }   
    }
}
