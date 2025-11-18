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
        public byte[] report_pdf { get; set; }

        public DateTime ReportDate { get; set; }

        public string report_type { get; set; }
        public string description { get; set; }

    }
}
