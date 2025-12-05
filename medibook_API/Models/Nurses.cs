using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Nurses
    {

        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("nurse_id")]
        public int nurse_id { get; set; }

        public string bio { get; set; } = string.Empty;
        [ForeignKey("Users")]
        [Column(TypeName = "INT")]
        [Required]
        public int user_id { get; set; }
        public Users Users { get; set; }
        public ICollection<Appointments> Appointments { get; set; }
    }
}
