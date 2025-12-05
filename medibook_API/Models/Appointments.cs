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

        public string? status { get; set; }

        public DateTime appointment_date { get; set; }
        public string? notes { get; set; }
        public string? medicine { get; set; }

        public DateTime create_date { get; set; } = DateTime.Now;

        [ForeignKey("Patients")]
        [Column(TypeName = "INT")]
        [Required]
        public int patient_id { get; set; }
        public virtual Users Patients { get; set; } // Changed from Users to Patients

        [ForeignKey("Doctors")]
        [Column(TypeName = "INT")]
        [Required]
        public int doctor_id { get; set; }
        public virtual Doctors Doctors { get; set; }

        // Remove [Required] from nullable foreign keys
        [ForeignKey("Nurses")]
        [Column(TypeName = "INT")]
        public int? nurse_id { get; set; } // Nullable and no [Required]
        public virtual Nurses Nurses { get; set; }

        [ForeignKey("Rooms")]
        [Column(TypeName = "INT")]
        public int? room_id { get; set; } // Nullable and no [Required]
        public virtual Rooms Rooms { get; set; }

        public virtual FeedBacks FeedBacks { get; set; }
    }
}