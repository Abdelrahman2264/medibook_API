using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Doctors
    {

        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("doctor_id")]
        public int doctor_id { get; set; }

        public string bio { get; set; } = string.Empty;
        public string specialization { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public int experience_years { get; set; }
        [ForeignKey("Users")]
        [Column(TypeName = "INT")]
        [Required]
        public int user_id { get; set; }
        public Users Users { get; set; }
        public ICollection<Appointments> Appointments { get; set; }
        public ICollection<FeedBacks> FeedBacks { get; set; }

    }
}
