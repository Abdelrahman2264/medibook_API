using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Notifications
    {

        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("notification_id")]
        public int notification_id { get; set; }

        public string message { get; set; } = string.Empty;
        public bool is_read { get; set; } = false;
        public DateTime create_date { get; set; } = DateTime.Now;
        public DateTime read_date { get; set; } = DateTime.Now;

        [ForeignKey("Users")]
        [Column(TypeName = "INT")]
        [Required]
        public int user_id { get; set; }
        public Users Users { get; set; }

    }
}
