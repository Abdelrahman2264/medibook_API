namespace medibook_API.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Reports
    {
        [Key]
        [Column(TypeName = "INT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("report_id")]
        public int report_id { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? report_file { get; set; }

        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string file_format { get; set; } = string.Empty; // "PDF" or "Excel"

        public DateTime ReportDate { get; set; }

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string report_type { get; set; } = string.Empty; // "Nurses", "Users", "Patients", "Appointments", "Feedbacks", "Summary"

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        public string? period_type { get; set; } // "Day", "Week", "Month", "Year", null for full reports

        [Column(TypeName = "varchar(500)")]
        [MaxLength(500)]
        public string description { get; set; } = string.Empty;

        [Column(TypeName = "varchar(255)")]
        [MaxLength(255)]
        public string file_name { get; set; } = string.Empty;

    }
}
