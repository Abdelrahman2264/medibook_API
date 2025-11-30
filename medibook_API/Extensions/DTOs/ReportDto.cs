using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class ReportDto
    {
        [JsonPropertyName("reportId")]
        public int ReportId { get; set; }
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;
        [JsonPropertyName("fileFormat")]
        public string FileFormat { get; set; } = string.Empty; // "PDF" or "Excel"
        [JsonPropertyName("reportDate")]
        public DateTime ReportDate { get; set; }
        [JsonPropertyName("reportType")]
        public string ReportType { get; set; } = string.Empty; // "Nurses", "Users", "Patients", "Appointments", "Feedbacks", "Summary"
        [JsonPropertyName("periodType")]
        public string? PeriodType { get; set; } // "Day", "Week", "Month", "Year", null for full reports
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("fileContent")]
        public byte[]? FileContent { get; set; }
    }
}

