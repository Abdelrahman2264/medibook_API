using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> logger;
        private readonly IReportRepository reportRepository;

        public ReportsController(ILogger<ReportsController> logger, IReportRepository reportRepository)
        {
            this.logger = logger;
            this.reportRepository = reportRepository;
        }

        // GET: /api/Reports/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ReportListDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                var reports = await reportRepository.GetAllReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all reports.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Reports/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReportById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid report ID.");

                var report = await reportRepository.GetReportByIdAsync(id);
                if (report == null)
                    return NotFound($"Report with ID {id} not found.");

                string contentType;
                switch (report.FileFormat.ToUpper())
                {
                    case "PDF":
                        contentType = "application/pdf";
                        break;
                    case "CSV":
                        contentType = "text/csv";
                        break;
                    case "EXCEL":
                    case "XLSX":
                    default:
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                }

                return File(report.FileContent!, contentType, report.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving report with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/nurses
        [HttpPost("generate/nurses")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateNursesReport([FromBody] CreateReportDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FileFormat) || 
                    (dto.FileFormat.ToUpper() != "PDF" && dto.FileFormat.ToUpper() != "EXCEL" && 
                     dto.FileFormat.ToUpper() != "XLSX" && dto.FileFormat.ToUpper() != "CSV"))
                    return BadRequest("File format must be 'PDF', 'Excel', 'XLSX', or 'CSV'.");

                var report = await reportRepository.GenerateNursesReportAsync(dto.FileFormat);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating nurses report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/doctors
        [HttpPost("generate/doctors")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateDoctorsReport([FromBody] CreateReportDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FileFormat) || 
                    (dto.FileFormat.ToUpper() != "PDF" && dto.FileFormat.ToUpper() != "EXCEL" && 
                     dto.FileFormat.ToUpper() != "XLSX" && dto.FileFormat.ToUpper() != "CSV"))
                    return BadRequest("File format must be 'PDF', 'Excel', 'XLSX', or 'CSV'.");

                var report = await reportRepository.GenerateDoctorsReportAsync(dto.FileFormat);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating doctors report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/users
        [HttpPost("generate/users")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateUsersReport([FromBody] CreateReportDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FileFormat) || 
                    (dto.FileFormat.ToUpper() != "PDF" && dto.FileFormat.ToUpper() != "EXCEL" && 
                     dto.FileFormat.ToUpper() != "XLSX" && dto.FileFormat.ToUpper() != "CSV"))
                    return BadRequest("File format must be 'PDF', 'Excel', 'XLSX', or 'CSV'.");

                var report = await reportRepository.GenerateUsersReportAsync(dto.FileFormat);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating users report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/patients
        [HttpPost("generate/patients")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GeneratePatientsReport([FromBody] CreateReportDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FileFormat) || 
                    (dto.FileFormat.ToUpper() != "PDF" && dto.FileFormat.ToUpper() != "EXCEL" && 
                     dto.FileFormat.ToUpper() != "XLSX" && dto.FileFormat.ToUpper() != "CSV"))
                    return BadRequest("File format must be 'PDF', 'Excel', 'XLSX', or 'CSV'.");

                var report = await reportRepository.GeneratePatientsReportAsync(dto.FileFormat);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating patients report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/appointments
        [HttpPost("generate/appointments")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateAppointmentsReport([FromBody] CreateReportDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FileFormat) || 
                    (dto.FileFormat.ToUpper() != "PDF" && dto.FileFormat.ToUpper() != "EXCEL" && 
                     dto.FileFormat.ToUpper() != "XLSX" && dto.FileFormat.ToUpper() != "CSV"))
                    return BadRequest("File format must be 'PDF', 'Excel', 'XLSX', or 'CSV'.");

                var report = await reportRepository.GenerateAppointmentsReportAsync(dto.FileFormat);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating appointments report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/feedbacks
        [HttpPost("generate/feedbacks")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateFeedbacksReport([FromBody] CreateReportDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.FileFormat) || 
                    (dto.FileFormat.ToUpper() != "PDF" && dto.FileFormat.ToUpper() != "EXCEL" && 
                     dto.FileFormat.ToUpper() != "XLSX" && dto.FileFormat.ToUpper() != "CSV"))
                    return BadRequest("File format must be 'PDF', 'Excel', 'XLSX', or 'CSV'.");

                var report = await reportRepository.GenerateFeedbacksReportAsync(dto.FileFormat);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating feedbacks report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Reports/generate/summary
        [HttpPost("generate/summary")]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateSummaryReport([FromBody] CreateReportDto dto)
        {
            try
            {
                // Normalize file format
                var normalizedFormat = string.IsNullOrEmpty(dto.FileFormat) ? "" : dto.FileFormat.ToUpper();
                if (normalizedFormat == "EXCEL")
                {
                    normalizedFormat = "XLSX";
                }

                if (string.IsNullOrEmpty(normalizedFormat) || 
                    (normalizedFormat != "PDF" && normalizedFormat != "XLSX" && normalizedFormat != "CSV"))
                {
                    return BadRequest("File format must be 'PDF', 'XLSX', or 'CSV'.");
                }

                if (string.IsNullOrEmpty(dto.PeriodType) || 
                    (dto.PeriodType.ToUpper() != "DAY" && dto.PeriodType.ToUpper() != "WEEK" && 
                     dto.PeriodType.ToUpper() != "MONTH" && dto.PeriodType.ToUpper() != "YEAR"))
                {
                    return BadRequest("Period type must be 'Day', 'Week', 'Month', or 'Year'.");
                }

                var report = await reportRepository.GenerateSummaryReportAsync(normalizedFormat, dto.PeriodType);
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while generating summary report.");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: /api/Reports/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid report ID.");

                var result = await reportRepository.DeleteReportAsync(id);
                if (!result)
                    return NotFound($"Report with ID {id} not found.");

                return Ok(new { Message = "Report deleted successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while deleting report with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

