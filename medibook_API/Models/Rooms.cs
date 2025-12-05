using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace medibook_API.Models
{
    public class Rooms
    {
        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("room_id")]
        public int room_id { get; set; }

        public string room_name { get; set; }
        public string room_type { get; set; }
        public bool is_active { get; set; }
        public DateTime create_date { get; set;  }
        [JsonIgnore]
        public ICollection<Appointments> ? Appointments { get; set; }

    }
}
