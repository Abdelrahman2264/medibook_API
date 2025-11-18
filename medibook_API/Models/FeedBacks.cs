using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class FeedBacks
    {
        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("feedback_id")]
        public int feedback_id { get; set; }

        public string comment { get; set; } = string.Empty;
        public int rate { get; set; } 
         public DateTime feedback_date { get; set; } = DateTime.Now;
        public string  doctor_reply { get; set; } = string.Empty;
        public DateTime reply_date { get; set; } 
        public bool is_favourite { get; set; } = false;

        [ForeignKey("Patients")]
        [Column(TypeName = "INT")]
        [Required]
        public int patient_id { get; set; }
        public Users Patients { get; set; }
        [ForeignKey("Doctors")]
        [Column(TypeName = "INT")]
        [Required]
        public int doctor_id { get; set; }
        public Doctors Doctors { get; set; }
        [ForeignKey("Appointments")]
        [Column(TypeName = "INT")]
        [Required]
        public int appointment_id { get; set; }
        public Appointments Appointments { get; set; }


    }
}
