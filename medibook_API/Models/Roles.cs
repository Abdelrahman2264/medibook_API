using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace medibook_API.Models
{
    public class Roles
    {
        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("role_id")]
        public int role_id { get; set; }
        public string role_name { get; set; } = string.Empty;
        public DateTime create_date { get; set; } = DateTime.Now;

        public ICollection<Users> Users { get; set; }
    }
}
