using System.Text.Json.Serialization;

namespace medibook_API.Extensions.DTOs
{
    public class ReportListDto
    {
        [JsonPropertyName("reportId")]
        public int ReportId { get; set; }
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;
        [JsonPropertyName("fileFormat")]
        public string FileFormat { get; set; } = string.Empty;
        [JsonPropertyName("reportDate")]
        public DateTime ReportDate { get; set; }
        [JsonPropertyName("reportType")]
        public string ReportType { get; set; } = string.Empty;
        [JsonPropertyName("periodType")]
        public string? PeriodType { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}

