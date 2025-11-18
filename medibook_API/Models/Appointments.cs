using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Appointments
    {
        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("appointment_id")]
        public int appointment_id { get; set; }

        public string status { get; set; } = string.Empty;

        public DateTime appointment_date { get; set; }
        public string notes { get; set; } = string.Empty;
        public string medicine { get; set; } = string.Empty;

        public DateTime create_date { get; set; } = DateTime.Now;

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
        [ForeignKey("Nurses")]
        [Column(TypeName = "INT")]
        [Required]
        public int nurse_id { get; set; }
        public Nurses Nurses { get; set; }
        [ForeignKey("Rooms")]
        [Column(TypeName = "INT")]
        [Required]
        public int room_id { get; set; }
        public Rooms Rooms { get; set; }

        public FeedBacks FeedBacks { get; set; }



    }
}
