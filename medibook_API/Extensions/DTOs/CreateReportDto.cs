using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class CreateReportDto
    {
        [JsonPropertyName("reportType")]
        public string ReportType { get; set; } = string.Empty; // "Nurses", "Users", "Patients", "Appointments", "Feedbacks", "Summary"
        [JsonPropertyName("fileFormat")]
        public string FileFormat { get; set; } = string.Empty; // "PDF" or "Excel"
        [JsonPropertyName("periodType")]
        public string? PeriodType { get; set; } // "Day", "Week", "Month", "Year", null for full reports
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}

