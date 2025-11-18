using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Logs
    {
        [Key]
        [Column(TypeName = "INT")]
        [DataType(DataType.Text)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("log_id")]
        public int log_id { get; set; }
        public string action_type { get; set; }
        public string log_type { get; set; }
        public DateTime log_date { get; set; }
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "INT")]
        [ForeignKey("Users")]
        public int user_id { get; set; }
        public Users? Users { get; set; }

    }
}
