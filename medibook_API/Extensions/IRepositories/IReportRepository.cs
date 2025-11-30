using medibook_API.Extensions.DTOs;

namespace medibook_API.Extensions.IRepositories
{
    public interface IReportRepository
    {
        Task<ReportDto> GenerateNursesReportAsync(string fileFormat);
        Task<ReportDto> GenerateDoctorsReportAsync(string fileFormat);
        Task<ReportDto> GenerateUsersReportAsync(string fileFormat);
        Task<ReportDto> GeneratePatientsReportAsync(string fileFormat);
        Task<ReportDto> GenerateAppointmentsReportAsync(string fileFormat);
        Task<ReportDto> GenerateFeedbacksReportAsync(string fileFormat);
        Task<ReportDto> GenerateSummaryReportAsync(string fileFormat, string periodType);
        Task<ReportDto> SaveReportAsync(ReportDto reportDto);
        Task<IEnumerable<ReportListDto>> GetAllReportsAsync();
        Task<ReportDto?> GetReportByIdAsync(int reportId);
        Task<bool> DeleteReportAsync(int reportId);
    }
}
