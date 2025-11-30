using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;
using System.Text;

namespace medibook_API.Extensions.Repositories
{
    // Summary Report Data Classes
    public class TopFeedbackData
    {
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime FeedbackDate { get; set; }
    }

    public class BestDoctorData
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public double AverageRating { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalFeedbacks { get; set; }
    }

    public class BestNurseData
    {
        public int NurseId { get; set; }
        public string NurseName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
    }

    public class MostPatientData
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
    }

    public class SummaryReportData
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalNurses { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public List<TopFeedbackData> TopFeedbacks { get; set; } = new();
        public List<BestDoctorData> BestDoctors { get; set; } = new();
        public List<BestNurseData> BestNurses { get; set; } = new();
        public List<MostPatientData> MostPatients { get; set; } = new();
    }

    public class ReportRepository : IReportRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<ReportRepository> logger;
        private readonly ILogRepository logRepository;

        public ReportRepository(
            Medibook_Context database,
            ILogger<ReportRepository> logger,
            ILogRepository logRepository)
        {
            this.database = database;
            this.logger = logger;
            this.logRepository = logRepository;
            QuestPDF.Settings.License = LicenseType.Community;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ReportDto> GenerateNursesReportAsync(string fileFormat)
        {
            try
            {
                var nurses = await database.Nurses
                    .Include(n => n.Users)
                    .ThenInclude(u => u.Role)
                    .Where(n => n.Users.is_active)
                    .ToListAsync();

                var description = $"Nurses Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Nurses_Report_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GenerateNursesExcel(nurses);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GenerateNursesCsv(nurses);
                }
                else
                {
                    fileContent = GenerateNursesPdf(nurses);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Nurses",
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating nurses report");
                await logRepository.CreateLogAsync("Generate Nurses Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> GenerateDoctorsReportAsync(string fileFormat)
        {
            try
            {
                var doctors = await database.Doctors
                    .Include(d => d.Users)
                    .ThenInclude(u => u.Role)
                    .Where(d => d.Users.is_active)
                    .ToListAsync();

                var description = $"Doctors Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Doctors_Report_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GenerateDoctorsExcel(doctors);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GenerateDoctorsCsv(doctors);
                }
                else
                {
                    fileContent = GenerateDoctorsPdf(doctors);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Doctors",
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating doctors report");
                await logRepository.CreateLogAsync("Generate Doctors Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> GenerateUsersReportAsync(string fileFormat)
        {
            try
            {
                var users = await database.Users
                    .Include(u => u.Role)
                    .ToListAsync();

                var description = $"Users Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Users_Report_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GenerateUsersExcel(users);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GenerateUsersCsv(users);
                }
                else
                {
                    fileContent = GenerateUsersPdf(users);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Users",
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating users report");
                await logRepository.CreateLogAsync("Generate Users Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> GeneratePatientsReportAsync(string fileFormat)
        {
            try
            {
                var patients = await database.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.role_name.ToLower() == "user" && u.is_active)
                    .ToListAsync();

                var description = $"Patients Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Patients_Report_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GeneratePatientsExcel(patients);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GeneratePatientsCsv(patients);
                }
                else
                {
                    fileContent = GeneratePatientsPdf(patients);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Patients",
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating patients report");
                await logRepository.CreateLogAsync("Generate Patients Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> GenerateAppointmentsReportAsync(string fileFormat)
        {
            try
            {
                var appointments = await database.Appointments
                    .Include(a => a.Patients)
                    .Include(a => a.Doctors)
                    .ThenInclude(d => d.Users)
                    .Include(a => a.Nurses)
                    .ThenInclude(n => n.Users)
                    .Include(a => a.Rooms)
                    .ToListAsync();

                var description = $"Appointments Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Appointments_Report_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GenerateAppointmentsExcel(appointments);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GenerateAppointmentsCsv(appointments);
                }
                else
                {
                    fileContent = GenerateAppointmentsPdf(appointments);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Appointments",
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating appointments report");
                await logRepository.CreateLogAsync("Generate Appointments Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> GenerateFeedbacksReportAsync(string fileFormat)
        {
            try
            {
                var feedbacks = await database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                    .ThenInclude(d => d.Users)
                    .Include(f => f.Appointments)
                    .ToListAsync();

                var description = $"Feedbacks Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Feedbacks_Report_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GenerateFeedbacksExcel(feedbacks);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GenerateFeedbacksCsv(feedbacks);
                }
                else
                {
                    fileContent = GenerateFeedbacksPdf(feedbacks);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Feedbacks",
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating feedbacks report");
                await logRepository.CreateLogAsync("Generate Feedbacks Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> GenerateSummaryReportAsync(string fileFormat, string periodType)
        {
            try
            {
                DateTime startDate, endDate;
                GetPeriodDates(periodType, out startDate, out endDate);

                // Get comprehensive summary data
                var totalUsers = await database.Users.CountAsync();
                var activeUsers = await database.Users.CountAsync(u => u.is_active);
                var totalNurses = await database.Nurses.CountAsync(n => n.Users.is_active);
                var totalDoctors = await database.Doctors.CountAsync(d => d.Users.is_active);
                var totalPatients = await database.Users.CountAsync(u => u.Role.role_name.ToLower() == "user" && u.is_active);
                var totalAppointments = await database.Appointments.CountAsync(a => a.appointment_date >= startDate && a.appointment_date <= endDate);
                var totalFeedbacks = await database.FeedBacks.CountAsync(f => f.feedback_date >= startDate && f.feedback_date <= endDate);
                
                // Get average rating
                var averageRating = await database.FeedBacks
                    .Where(f => f.feedback_date >= startDate && f.feedback_date <= endDate)
                    .Select(f => (double?)f.rate)
                    .AverageAsync() ?? 0.0;

                // Top 5 Feedbacks
                var topFeedbacksData = await database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                    .ThenInclude(d => d.Users)
                    .Where(f => f.feedback_date >= startDate && f.feedback_date <= endDate)
                    .OrderByDescending(f => f.rate)
                    .ThenByDescending(f => f.feedback_date)
                    .Take(5)
                    .ToListAsync();

                var topFeedbacks = topFeedbacksData.Select(f => new
                {
                    PatientName = $"{f.Patients.first_name} {f.Patients.last_name}",
                    DoctorName = $"{f.Doctors.Users.first_name} {f.Doctors.Users.last_name}",
                    Rate = f.rate,
                    Comment = f.comment,
                    FeedbackDate = f.feedback_date
                }).ToList();

                // Best Doctors (by average rating and appointment count)
                var bestDoctorsData = await database.Doctors
                    .Include(d => d.Users)
                    .Include(d => d.FeedBacks)
                    .Include(d => d.Appointments)
                    .Where(d => d.Users.is_active)
                    .ToListAsync();

                var bestDoctors = bestDoctorsData
                    .Select(d => new
                    {
                        DoctorId = d.doctor_id,
                        DoctorName = $"{d.Users.first_name} {d.Users.last_name}",
                        Specialization = d.specialization,
                        ExperienceYears = d.experience_years,
                        AverageRating = d.FeedBacks.Any() ? d.FeedBacks.Average(f => (double?)f.rate) ?? 0 : 0,
                        TotalAppointments = d.Appointments.Count(a => a.appointment_date >= startDate && a.appointment_date <= endDate),
                        TotalFeedbacks = d.FeedBacks.Count(f => f.feedback_date >= startDate && f.feedback_date <= endDate)
                    })
                    .OrderByDescending(d => d.AverageRating)
                    .ThenByDescending(d => d.TotalAppointments)
                    .Take(5)
                    .ToList();

                // Best Nurses (by appointment count)
                var bestNursesData = await database.Nurses
                    .Include(n => n.Users)
                    .Include(n => n.Appointments)
                    .Where(n => n.Users.is_active)
                    .ToListAsync();

                var bestNurses = bestNursesData
                    .Select(n => new
                    {
                        NurseId = n.nurse_id,
                        NurseName = $"{n.Users.first_name} {n.Users.last_name}",
                        TotalAppointments = n.Appointments.Count(a => a.appointment_date >= startDate && a.appointment_date <= endDate)
                    })
                    .OrderByDescending(n => n.TotalAppointments)
                    .Take(5)
                    .ToList();

                // Most Active Patients (by appointment count)
                var mostPatientsData = await database.Users
                    .Include(u => u.Role)
                    .Include(u => u.Appointments)
                    .Where(u => u.Role.role_name.ToLower() == "user" && u.is_active)
                    .ToListAsync();

                var mostPatients = mostPatientsData
                    .Select(u => new
                    {
                        PatientId = u.user_id,
                        PatientName = $"{u.first_name} {u.last_name}",
                        Email = u.email,
                        TotalAppointments = u.Appointments.Count(a => a.appointment_date >= startDate && a.appointment_date <= endDate)
                    })
                    .OrderByDescending(p => p.TotalAppointments)
                    .Take(5)
                    .ToList();

                var summary = new SummaryReportData
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalNurses = totalNurses,
                    TotalDoctors = totalDoctors,
                    TotalPatients = totalPatients,
                    TotalAppointments = totalAppointments,
                    TotalFeedbacks = totalFeedbacks,
                    AverageRating = Math.Round(averageRating, 2),
                    PeriodStart = startDate,
                    PeriodEnd = endDate,
                    TopFeedbacks = topFeedbacks.Select(f => new TopFeedbackData
                    {
                        PatientName = f.PatientName,
                        DoctorName = f.DoctorName,
                        Rate = f.Rate,
                        Comment = f.Comment,
                        FeedbackDate = f.FeedbackDate
                    }).ToList(),
                    BestDoctors = bestDoctors.Select(d => new BestDoctorData
                    {
                        DoctorId = d.DoctorId,
                        DoctorName = d.DoctorName,
                        Specialization = d.Specialization,
                        ExperienceYears = d.ExperienceYears,
                        AverageRating = d.AverageRating,
                        TotalAppointments = d.TotalAppointments,
                        TotalFeedbacks = d.TotalFeedbacks
                    }).ToList(),
                    BestNurses = bestNurses.Select(n => new BestNurseData
                    {
                        NurseId = n.NurseId,
                        NurseName = n.NurseName,
                        TotalAppointments = n.TotalAppointments
                    }).ToList(),
                    MostPatients = mostPatients.Select(p => new MostPatientData
                    {
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        Email = p.Email,
                        TotalAppointments = p.TotalAppointments
                    }).ToList()
                };

                var description = $"{periodType} Summary Report - Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                var fileExtension = GetFileExtension(fileFormat);
                var fileName = $"Summary_Report_{periodType}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";

                byte[] fileContent;
                if (fileFormat.ToUpper() == "EXCEL" || fileFormat.ToUpper() == "XLSX")
                {
                    fileContent = GenerateSummaryExcel(summary);
                }
                else if (fileFormat.ToUpper() == "CSV")
                {
                    fileContent = GenerateSummaryCsv(summary);
                }
                else
                {
                    fileContent = GenerateSummaryPdf(summary);
                }

                var reportDto = new ReportDto
                {
                    FileName = fileName,
                    FileFormat = fileFormat.ToUpper() == "EXCEL" ? "XLSX" : fileFormat.ToUpper(),
                    ReportDate = DateTime.Now,
                    ReportType = "Summary",
                    PeriodType = periodType,
                    Description = description,
                    FileContent = fileContent
                };

                return await SaveReportAsync(reportDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating summary report");
                await logRepository.CreateLogAsync("Generate Summary Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto> SaveReportAsync(ReportDto reportDto)
        {
            try
            {
                var report = new Reports
                {
                    report_file = reportDto.FileContent,
                    file_format = reportDto.FileFormat,
                    ReportDate = reportDto.ReportDate,
                    report_type = reportDto.ReportType,
                    period_type = reportDto.PeriodType,
                    description = reportDto.Description,
                    file_name = reportDto.FileName
                };

                database.Reports.Add(report);
                await database.SaveChangesAsync();

                reportDto.ReportId = report.report_id;
                logger.LogInformation("Report saved successfully with ID: {ReportId}", report.report_id);
                await logRepository.CreateLogAsync("Save Report", "Success", $"Report {report.report_id} saved.");

                return reportDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving report");
                await logRepository.CreateLogAsync("Save Report", "Error", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ReportListDto>> GetAllReportsAsync()
        {
            try
            {
                var reports = await database.Reports
                    .OrderByDescending(r => r.ReportDate)
                    .Select(r => new ReportListDto
                    {
                        ReportId = r.report_id,
                        FileName = r.file_name,
                        FileFormat = r.file_format,
                        ReportDate = r.ReportDate,
                        ReportType = r.report_type,
                        PeriodType = r.period_type,
                        Description = r.description
                    })
                    .ToListAsync();

                return reports;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all reports");
                await logRepository.CreateLogAsync("Get All Reports", "Error", ex.Message);
                throw;
            }
        }

        public async Task<ReportDto?> GetReportByIdAsync(int reportId)
        {
            try
            {
                var report = await database.Reports.FindAsync(reportId);
                if (report == null)
                    return null;

                return new ReportDto
                {
                    ReportId = report.report_id,
                    FileName = report.file_name,
                    FileFormat = report.file_format,
                    ReportDate = report.ReportDate,
                    ReportType = report.report_type,
                    PeriodType = report.period_type,
                    Description = report.description,
                    FileContent = report.report_file
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving report by ID");
                await logRepository.CreateLogAsync("Get Report By ID", "Error", ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            try
            {
                var report = await database.Reports.FindAsync(reportId);
                if (report == null)
                {
                    logger.LogWarning("Report with ID {ReportId} not found", reportId);
                    return false;
                }

                database.Reports.Remove(report);
                await database.SaveChangesAsync();

                logger.LogInformation("Report {ReportId} deleted successfully", reportId);
                await logRepository.CreateLogAsync("Delete Report", "Success", $"Report {reportId} deleted.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting report");
                await logRepository.CreateLogAsync("Delete Report", "Error", ex.Message);
                throw;
            }
        }

        // Helper methods for period calculation
        private void GetPeriodDates(string periodType, out DateTime startDate, out DateTime endDate)
        {
            var now = DateTime.Now;
            endDate = now;

            switch (periodType.ToUpper())
            {
                case "DAY":
                    startDate = now.Date;
                    endDate = now.Date.AddDays(1).AddTicks(-1);
                    break;
                case "WEEK":
                    startDate = now.Date.AddDays(-(int)now.DayOfWeek);
                    endDate = startDate.AddDays(7).AddTicks(-1);
                    break;
                case "MONTH":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
                case "YEAR":
                    startDate = new DateTime(now.Year, 1, 1);
                    endDate = startDate.AddYears(1).AddTicks(-1);
                    break;
                default:
                    startDate = DateTime.MinValue;
                    endDate = DateTime.MaxValue;
                    break;
            }
        }

        // Helper method to get file extension
        private string GetFileExtension(string fileFormat)
        {
            switch (fileFormat.ToUpper())
            {
                case "EXCEL":
                case "XLSX":
                    return ".xlsx";
                case "CSV":
                    return ".csv";
                case "PDF":
                default:
                    return ".pdf";
            }
        }

        // Excel Generation Methods
        private byte[] GenerateNursesExcel(List<Nurses> nurses)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Nurses");

            // Headers
            worksheet.Cells[1, 1].Value = "Nurse ID";
            worksheet.Cells[1, 2].Value = "First Name";
            worksheet.Cells[1, 3].Value = "Last Name";
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Mobile Phone";
            worksheet.Cells[1, 6].Value = "Bio";
            worksheet.Cells[1, 7].Value = "Gender";
            worksheet.Cells[1, 8].Value = "Date of Birth";
            worksheet.Cells[1, 9].Value = "Create Date";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 9])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            // Data
            int row = 2;
            foreach (var nurse in nurses)
            {
                worksheet.Cells[row, 1].Value = nurse.nurse_id;
                worksheet.Cells[row, 2].Value = nurse.Users.first_name;
                worksheet.Cells[row, 3].Value = nurse.Users.last_name;
                worksheet.Cells[row, 4].Value = nurse.Users.email;
                worksheet.Cells[row, 5].Value = nurse.Users.mobile_phone;
                worksheet.Cells[row, 6].Value = nurse.bio;
                worksheet.Cells[row, 7].Value = nurse.Users.gender;
                worksheet.Cells[row, 8].Value = nurse.Users.date_of_birth.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 9].Value = nurse.Users.create_date.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private byte[] GenerateUsersExcel(List<Users> users)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users");

            worksheet.Cells[1, 1].Value = "User ID";
            worksheet.Cells[1, 2].Value = "First Name";
            worksheet.Cells[1, 3].Value = "Last Name";
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Mobile Phone";
            worksheet.Cells[1, 6].Value = "Gender";
            worksheet.Cells[1, 7].Value = "Marital Status";
            worksheet.Cells[1, 8].Value = "Role";
            worksheet.Cells[1, 9].Value = "Is Active";
            worksheet.Cells[1, 10].Value = "Date of Birth";
            worksheet.Cells[1, 11].Value = "Create Date";

            using (var range = worksheet.Cells[1, 1, 1, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 2;
            foreach (var user in users)
            {
                worksheet.Cells[row, 1].Value = user.user_id;
                worksheet.Cells[row, 2].Value = user.first_name;
                worksheet.Cells[row, 3].Value = user.last_name;
                worksheet.Cells[row, 4].Value = user.email;
                worksheet.Cells[row, 5].Value = user.mobile_phone;
                worksheet.Cells[row, 6].Value = user.gender;
                worksheet.Cells[row, 7].Value = user.mitrial_status;
                worksheet.Cells[row, 8].Value = user.Role?.role_name ?? "N/A";
                worksheet.Cells[row, 9].Value = user.is_active ? "Yes" : "No";
                worksheet.Cells[row, 10].Value = user.date_of_birth.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 11].Value = user.create_date.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private byte[] GeneratePatientsExcel(List<Users> patients)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Patients");

            worksheet.Cells[1, 1].Value = "Patient ID";
            worksheet.Cells[1, 2].Value = "First Name";
            worksheet.Cells[1, 3].Value = "Last Name";
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Mobile Phone";
            worksheet.Cells[1, 6].Value = "Gender";
            worksheet.Cells[1, 7].Value = "Marital Status";
            worksheet.Cells[1, 8].Value = "Date of Birth";
            worksheet.Cells[1, 9].Value = "Create Date";

            using (var range = worksheet.Cells[1, 1, 1, 9])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 2;
            foreach (var patient in patients)
            {
                worksheet.Cells[row, 1].Value = patient.user_id;
                worksheet.Cells[row, 2].Value = patient.first_name;
                worksheet.Cells[row, 3].Value = patient.last_name;
                worksheet.Cells[row, 4].Value = patient.email;
                worksheet.Cells[row, 5].Value = patient.mobile_phone;
                worksheet.Cells[row, 6].Value = patient.gender;
                worksheet.Cells[row, 7].Value = patient.mitrial_status;
                worksheet.Cells[row, 8].Value = patient.date_of_birth.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 9].Value = patient.create_date.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private byte[] GenerateAppointmentsExcel(List<Appointments> appointments)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Appointments");

            worksheet.Cells[1, 1].Value = "Appointment ID";
            worksheet.Cells[1, 2].Value = "Patient Name";
            worksheet.Cells[1, 3].Value = "Doctor Name";
            worksheet.Cells[1, 4].Value = "Nurse Name";
            worksheet.Cells[1, 5].Value = "Room";
            worksheet.Cells[1, 6].Value = "Appointment Date";
            worksheet.Cells[1, 7].Value = "Status";
            worksheet.Cells[1, 8].Value = "Notes";
            worksheet.Cells[1, 9].Value = "Medicine";
            worksheet.Cells[1, 10].Value = "Create Date";

            using (var range = worksheet.Cells[1, 1, 1, 10])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 2;
            foreach (var appointment in appointments)
            {
                worksheet.Cells[row, 1].Value = appointment.appointment_id;
                worksheet.Cells[row, 2].Value = $"{appointment.Patients.first_name} {appointment.Patients.last_name}";
                worksheet.Cells[row, 3].Value = $"{appointment.Doctors.Users.first_name} {appointment.Doctors.Users.last_name}";
                worksheet.Cells[row, 4].Value = appointment.Nurses != null ? $"{appointment.Nurses.Users.first_name} {appointment.Nurses.Users.last_name}" : "N/A";
                worksheet.Cells[row, 5].Value = appointment.Rooms?.room_name ?? "N/A";
                worksheet.Cells[row, 6].Value = appointment.appointment_date.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 7].Value = appointment.status ?? "N/A";
                worksheet.Cells[row, 8].Value = appointment.notes ?? "N/A";
                worksheet.Cells[row, 9].Value = appointment.medicine ?? "N/A";
                worksheet.Cells[row, 10].Value = appointment.create_date.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private byte[] GenerateFeedbacksExcel(List<FeedBacks> feedbacks)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Feedbacks");

            worksheet.Cells[1, 1].Value = "Feedback ID";
            worksheet.Cells[1, 2].Value = "Patient Name";
            worksheet.Cells[1, 3].Value = "Doctor Name";
            worksheet.Cells[1, 4].Value = "Appointment ID";
            worksheet.Cells[1, 5].Value = "Rate";
            worksheet.Cells[1, 6].Value = "Comment";
            worksheet.Cells[1, 7].Value = "Doctor Reply";
            worksheet.Cells[1, 8].Value = "Is Favourite";
            worksheet.Cells[1, 9].Value = "Feedback Date";
            worksheet.Cells[1, 10].Value = "Reply Date";

            using (var range = worksheet.Cells[1, 1, 1, 10])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 2;
            foreach (var feedback in feedbacks)
            {
                worksheet.Cells[row, 1].Value = feedback.feedback_id;
                worksheet.Cells[row, 2].Value = $"{feedback.Patients.first_name} {feedback.Patients.last_name}";
                worksheet.Cells[row, 3].Value = $"{feedback.Doctors.Users.first_name} {feedback.Doctors.Users.last_name}";
                worksheet.Cells[row, 4].Value = feedback.appointment_id;
                worksheet.Cells[row, 5].Value = feedback.rate;
                worksheet.Cells[row, 6].Value = feedback.comment;
                worksheet.Cells[row, 7].Value = feedback.doctor_reply ?? "N/A";
                worksheet.Cells[row, 8].Value = feedback.is_favourite ? "Yes" : "No";
                worksheet.Cells[row, 9].Value = feedback.feedback_date.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 10].Value = feedback.reply_date?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private byte[] GenerateSummaryExcel(SummaryReportData summary)
        {
            using var package = new ExcelPackage();
            
            // Overview Sheet
            var overviewSheet = package.Workbook.Worksheets.Add("Overview");
            overviewSheet.Cells[1, 1].Value = "Metric";
            overviewSheet.Cells[1, 2].Value = "Value";

            using (var range = overviewSheet.Cells[1, 1, 1, 2])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 2;
            overviewSheet.Cells[row, 1].Value = "Total Users";
            overviewSheet.Cells[row, 2].Value = summary.TotalUsers;
            row++;

            overviewSheet.Cells[row, 1].Value = "Active Users";
            overviewSheet.Cells[row, 2].Value = summary.ActiveUsers;
            row++;

            overviewSheet.Cells[row, 1].Value = "Total Nurses";
            overviewSheet.Cells[row, 2].Value = summary.TotalNurses;
            row++;

            overviewSheet.Cells[row, 1].Value = "Total Doctors";
            overviewSheet.Cells[row, 2].Value = summary.TotalDoctors;
            row++;

            overviewSheet.Cells[row, 1].Value = "Total Patients";
            overviewSheet.Cells[row, 2].Value = summary.TotalPatients;
            row++;

            overviewSheet.Cells[row, 1].Value = "Total Appointments (Period)";
            overviewSheet.Cells[row, 2].Value = summary.TotalAppointments;
            row++;

            overviewSheet.Cells[row, 1].Value = "Total Feedbacks (Period)";
            overviewSheet.Cells[row, 2].Value = summary.TotalFeedbacks;
            row++;

            overviewSheet.Cells[row, 1].Value = "Average Rating";
            overviewSheet.Cells[row, 2].Value = summary.AverageRating;
            row++;

            overviewSheet.Cells[row, 1].Value = "Period Start";
            overviewSheet.Cells[row, 2].Value = summary.PeriodStart.ToString("yyyy-MM-dd HH:mm:ss");
            row++;

            overviewSheet.Cells[row, 1].Value = "Period End";
            overviewSheet.Cells[row, 2].Value = summary.PeriodEnd.ToString("yyyy-MM-dd HH:mm:ss");
            overviewSheet.Cells.AutoFitColumns();

            // Top Feedbacks Sheet
            var feedbacksSheet = package.Workbook.Worksheets.Add("Top Feedbacks");
            feedbacksSheet.Cells[1, 1].Value = "Patient Name";
            feedbacksSheet.Cells[1, 2].Value = "Doctor Name";
            feedbacksSheet.Cells[1, 3].Value = "Rate";
            feedbacksSheet.Cells[1, 4].Value = "Comment";
            feedbacksSheet.Cells[1, 5].Value = "Date";

            using (var range = feedbacksSheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            row = 2;
            foreach (var feedback in summary.TopFeedbacks)
            {
                feedbacksSheet.Cells[row, 1].Value = feedback.PatientName;
                feedbacksSheet.Cells[row, 2].Value = feedback.DoctorName;
                feedbacksSheet.Cells[row, 3].Value = feedback.Rate;
                feedbacksSheet.Cells[row, 4].Value = feedback.Comment;
                feedbacksSheet.Cells[row, 5].Value = feedback.FeedbackDate.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }
            feedbacksSheet.Cells.AutoFitColumns();

            // Best Doctors Sheet
            var doctorsSheet = package.Workbook.Worksheets.Add("Best Doctors");
            doctorsSheet.Cells[1, 1].Value = "Doctor Name";
            doctorsSheet.Cells[1, 2].Value = "Specialization";
            doctorsSheet.Cells[1, 3].Value = "Experience Years";
            doctorsSheet.Cells[1, 4].Value = "Average Rating";
            doctorsSheet.Cells[1, 5].Value = "Total Appointments";
            doctorsSheet.Cells[1, 6].Value = "Total Feedbacks";

            using (var range = doctorsSheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }

            row = 2;
            foreach (var doctor in summary.BestDoctors)
            {
                doctorsSheet.Cells[row, 1].Value = doctor.DoctorName;
                doctorsSheet.Cells[row, 2].Value = doctor.Specialization;
                doctorsSheet.Cells[row, 3].Value = doctor.ExperienceYears;
                doctorsSheet.Cells[row, 4].Value = Math.Round(doctor.AverageRating, 2);
                doctorsSheet.Cells[row, 5].Value = doctor.TotalAppointments;
                doctorsSheet.Cells[row, 6].Value = doctor.TotalFeedbacks;
                row++;
            }
            doctorsSheet.Cells.AutoFitColumns();

            // Best Nurses Sheet
            var nursesSheet = package.Workbook.Worksheets.Add("Best Nurses");
            nursesSheet.Cells[1, 1].Value = "Nurse Name";
            nursesSheet.Cells[1, 2].Value = "Total Appointments";

            using (var range = nursesSheet.Cells[1, 1, 1, 2])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
            }

            row = 2;
            foreach (var nurse in summary.BestNurses)
            {
                nursesSheet.Cells[row, 1].Value = nurse.NurseName;
                nursesSheet.Cells[row, 2].Value = nurse.TotalAppointments;
                row++;
            }
            nursesSheet.Cells.AutoFitColumns();

            // Most Active Patients Sheet
            var patientsSheet = package.Workbook.Worksheets.Add("Most Active Patients");
            patientsSheet.Cells[1, 1].Value = "Patient Name";
            patientsSheet.Cells[1, 2].Value = "Email";
            patientsSheet.Cells[1, 3].Value = "Total Appointments";

            using (var range = patientsSheet.Cells[1, 1, 1, 3])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSteelBlue);
            }

            row = 2;
            foreach (var patient in summary.MostPatients)
            {
                patientsSheet.Cells[row, 1].Value = patient.PatientName;
                patientsSheet.Cells[row, 2].Value = patient.Email;
                patientsSheet.Cells[row, 3].Value = patient.TotalAppointments;
                row++;
            }
            patientsSheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

        private byte[] GenerateDoctorsExcel(List<Doctors> doctors)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Doctors");

            worksheet.Cells[1, 1].Value = "Doctor ID";
            worksheet.Cells[1, 2].Value = "First Name";
            worksheet.Cells[1, 3].Value = "Last Name";
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Mobile Phone";
            worksheet.Cells[1, 6].Value = "Specialization";
            worksheet.Cells[1, 7].Value = "Type";
            worksheet.Cells[1, 8].Value = "Experience Years";
            worksheet.Cells[1, 9].Value = "Bio";
            worksheet.Cells[1, 10].Value = "Gender";
            worksheet.Cells[1, 11].Value = "Date of Birth";
            worksheet.Cells[1, 12].Value = "Create Date";

            using (var range = worksheet.Cells[1, 1, 1, 12])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 2;
            foreach (var doctor in doctors)
            {
                worksheet.Cells[row, 1].Value = doctor.doctor_id;
                worksheet.Cells[row, 2].Value = doctor.Users.first_name;
                worksheet.Cells[row, 3].Value = doctor.Users.last_name;
                worksheet.Cells[row, 4].Value = doctor.Users.email;
                worksheet.Cells[row, 5].Value = doctor.Users.mobile_phone;
                worksheet.Cells[row, 6].Value = doctor.specialization;
                worksheet.Cells[row, 7].Value = doctor.Type;
                worksheet.Cells[row, 8].Value = doctor.experience_years;
                worksheet.Cells[row, 9].Value = doctor.bio;
                worksheet.Cells[row, 10].Value = doctor.Users.gender;
                worksheet.Cells[row, 11].Value = doctor.Users.date_of_birth.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 12].Value = doctor.Users.create_date.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        // CSV Generation Methods
        private byte[] GenerateDoctorsCsv(List<Doctors> doctors)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Doctor ID,First Name,Last Name,Email,Mobile Phone,Specialization,Type,Experience Years,Bio,Gender,Date of Birth,Create Date");

            foreach (var doctor in doctors)
            {
                csv.AppendLine($"{doctor.doctor_id},\"{doctor.Users.first_name}\",\"{doctor.Users.last_name}\",\"{doctor.Users.email}\",\"{doctor.Users.mobile_phone}\",\"{doctor.specialization}\",\"{doctor.Type}\",{doctor.experience_years},\"{doctor.bio.Replace("\"", "\"\"")}\",\"{doctor.Users.gender}\",\"{doctor.Users.date_of_birth:yyyy-MM-dd}\",\"{doctor.Users.create_date:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateNursesCsv(List<Nurses> nurses)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Nurse ID,First Name,Last Name,Email,Mobile Phone,Bio,Gender,Date of Birth,Create Date");

            foreach (var nurse in nurses)
            {
                csv.AppendLine($"{nurse.nurse_id},\"{nurse.Users.first_name}\",\"{nurse.Users.last_name}\",\"{nurse.Users.email}\",\"{nurse.Users.mobile_phone}\",\"{nurse.bio.Replace("\"", "\"\"")}\",\"{nurse.Users.gender}\",\"{nurse.Users.date_of_birth:yyyy-MM-dd}\",\"{nurse.Users.create_date:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateUsersCsv(List<Users> users)
        {
            var csv = new StringBuilder();
            csv.AppendLine("User ID,First Name,Last Name,Email,Mobile Phone,Gender,Marital Status,Role,Is Active,Date of Birth,Create Date");

            foreach (var user in users)
            {
                csv.AppendLine($"{user.user_id},\"{user.first_name}\",\"{user.last_name}\",\"{user.email}\",\"{user.mobile_phone}\",\"{user.gender}\",\"{user.mitrial_status}\",\"{user.Role?.role_name ?? "N/A"}\",\"{(user.is_active ? "Yes" : "No")}\",\"{user.date_of_birth:yyyy-MM-dd}\",\"{user.create_date:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GeneratePatientsCsv(List<Users> patients)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Patient ID,First Name,Last Name,Email,Mobile Phone,Gender,Marital Status,Date of Birth,Create Date");

            foreach (var patient in patients)
            {
                csv.AppendLine($"{patient.user_id},\"{patient.first_name}\",\"{patient.last_name}\",\"{patient.email}\",\"{patient.mobile_phone}\",\"{patient.gender}\",\"{patient.mitrial_status}\",\"{patient.date_of_birth:yyyy-MM-dd}\",\"{patient.create_date:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateAppointmentsCsv(List<Appointments> appointments)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Appointment ID,Patient Name,Doctor Name,Nurse Name,Room,Appointment Date,Status,Notes,Medicine,Create Date");

            foreach (var appointment in appointments)
            {
                var patientName = $"{appointment.Patients.first_name} {appointment.Patients.last_name}";
                var doctorName = $"{appointment.Doctors.Users.first_name} {appointment.Doctors.Users.last_name}";
                var nurseName = appointment.Nurses != null ? $"{appointment.Nurses.Users.first_name} {appointment.Nurses.Users.last_name}" : "N/A";
                var roomName = appointment.Rooms?.room_name ?? "N/A";
                var notes = appointment.notes?.Replace("\"", "\"\"") ?? "N/A";
                var medicine = appointment.medicine?.Replace("\"", "\"\"") ?? "N/A";

                csv.AppendLine($"{appointment.appointment_id},\"{patientName}\",\"{doctorName}\",\"{nurseName}\",\"{roomName}\",\"{appointment.appointment_date:yyyy-MM-dd HH:mm:ss}\",\"{appointment.status ?? "N/A"}\",\"{notes}\",\"{medicine}\",\"{appointment.create_date:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateFeedbacksCsv(List<FeedBacks> feedbacks)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Feedback ID,Patient Name,Doctor Name,Appointment ID,Rate,Comment,Doctor Reply,Is Favourite,Feedback Date,Reply Date");

            foreach (var feedback in feedbacks)
            {
                var patientName = $"{feedback.Patients.first_name} {feedback.Patients.last_name}";
                var doctorName = $"{feedback.Doctors.Users.first_name} {feedback.Doctors.Users.last_name}";
                var comment = feedback.comment.Replace("\"", "\"\"");
                var doctorReply = feedback.doctor_reply?.Replace("\"", "\"\"") ?? "N/A";
                var replyDate = feedback.reply_date?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";

                csv.AppendLine($"{feedback.feedback_id},\"{patientName}\",\"{doctorName}\",{feedback.appointment_id},{feedback.rate},\"{comment}\",\"{doctorReply}\",\"{(feedback.is_favourite ? "Yes" : "No")}\",\"{feedback.feedback_date:yyyy-MM-dd HH:mm:ss}\",\"{replyDate}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateSummaryCsv(SummaryReportData summary)
        {
            var csv = new StringBuilder();
            
            // Overview Section
            csv.AppendLine("=== OVERVIEW ===");
            csv.AppendLine("Metric,Value");
            csv.AppendLine($"Total Users,{summary.TotalUsers}");
            csv.AppendLine($"Active Users,{summary.ActiveUsers}");
            csv.AppendLine($"Total Nurses,{summary.TotalNurses}");
            csv.AppendLine($"Total Doctors,{summary.TotalDoctors}");
            csv.AppendLine($"Total Patients,{summary.TotalPatients}");
            csv.AppendLine($"Total Appointments (Period),{summary.TotalAppointments}");
            csv.AppendLine($"Total Feedbacks (Period),{summary.TotalFeedbacks}");
            csv.AppendLine($"Average Rating,{summary.AverageRating}");
            csv.AppendLine($"Period Start,\"{summary.PeriodStart:yyyy-MM-dd HH:mm:ss}\"");
            csv.AppendLine($"Period End,\"{summary.PeriodEnd:yyyy-MM-dd HH:mm:ss}\"");
            csv.AppendLine();
            
            // Top Feedbacks
            csv.AppendLine("=== TOP 5 FEEDBACKS ===");
            csv.AppendLine("Patient Name,Doctor Name,Rate,Comment,Date");
            foreach (var feedback in summary.TopFeedbacks)
            {
                csv.AppendLine($"\"{feedback.PatientName}\",\"{feedback.DoctorName}\",{feedback.Rate},\"{feedback.Comment.Replace("\"", "\"\"")}\",\"{feedback.FeedbackDate:yyyy-MM-dd HH:mm:ss}\"");
            }
            csv.AppendLine();
            
            // Best Doctors
            csv.AppendLine("=== BEST 5 DOCTORS ===");
            csv.AppendLine("Doctor Name,Specialization,Experience Years,Average Rating,Total Appointments,Total Feedbacks");
            foreach (var doctor in summary.BestDoctors)
            {
                csv.AppendLine($"\"{doctor.DoctorName}\",\"{doctor.Specialization}\",{doctor.ExperienceYears},{Math.Round(doctor.AverageRating, 2)},{doctor.TotalAppointments},{doctor.TotalFeedbacks}");
            }
            csv.AppendLine();
            
            // Best Nurses
            csv.AppendLine("=== BEST 5 NURSES ===");
            csv.AppendLine("Nurse Name,Total Appointments");
            foreach (var nurse in summary.BestNurses)
            {
                csv.AppendLine($"\"{nurse.NurseName}\",{nurse.TotalAppointments}");
            }
            csv.AppendLine();
            
            // Most Active Patients
            csv.AppendLine("=== MOST ACTIVE 5 PATIENTS ===");
            csv.AppendLine("Patient Name,Email,Total Appointments");
            foreach (var patient in summary.MostPatients)
            {
                csv.AppendLine($"\"{patient.PatientName}\",\"{patient.Email}\",{patient.TotalAppointments}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        // PDF Generation Methods
        private byte[] GenerateNursesPdf(List<Nurses> nurses)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text("Nurses Report")
                        .Bold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("ID");
                                    header.Cell().Element(CellStyle).Text("Name");
                                    header.Cell().Element(CellStyle).Text("Email");
                                    header.Cell().Element(CellStyle).Text("Phone");
                                });

                                foreach (var nurse in nurses)
                                {
                                    table.Cell().Element(CellStyle).Text(nurse.nurse_id.ToString());
                                    table.Cell().Element(CellStyle).Text($"{nurse.Users.first_name} {nurse.Users.last_name}");
                                    table.Cell().Element(CellStyle).Text(nurse.Users.email);
                                    table.Cell().Element(CellStyle).Text(nurse.Users.mobile_phone);
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private byte[] GenerateUsersPdf(List<Users> users)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Text("Users Report")
                        .Bold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Name");
                                header.Cell().Element(CellStyle).Text("Email");
                                header.Cell().Element(CellStyle).Text("Role");
                            });

                            foreach (var user in users)
                            {
                                table.Cell().Element(CellStyle).Text(user.user_id.ToString());
                                table.Cell().Element(CellStyle).Text($"{user.first_name} {user.last_name}");
                                table.Cell().Element(CellStyle).Text(user.email);
                                table.Cell().Element(CellStyle).Text(user.Role?.role_name ?? "N/A");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private byte[] GeneratePatientsPdf(List<Users> patients)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text("Patients Report")
                        .Bold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Name");
                                header.Cell().Element(CellStyle).Text("Email");
                                header.Cell().Element(CellStyle).Text("Phone");
                            });

                            foreach (var patient in patients)
                            {
                                table.Cell().Element(CellStyle).Text(patient.user_id.ToString());
                                table.Cell().Element(CellStyle).Text($"{patient.first_name} {patient.last_name}");
                                table.Cell().Element(CellStyle).Text(patient.email);
                                table.Cell().Element(CellStyle).Text(patient.mobile_phone);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private byte[] GenerateAppointmentsPdf(List<Appointments> appointments)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Header()
                        .Text("Appointments Report")
                        .Bold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Patient");
                                header.Cell().Element(CellStyle).Text("Doctor");
                                header.Cell().Element(CellStyle).Text("Date");
                            });

                            foreach (var appointment in appointments)
                            {
                                table.Cell().Element(CellStyle).Text(appointment.appointment_id.ToString());
                                table.Cell().Element(CellStyle).Text($"{appointment.Patients.first_name} {appointment.Patients.last_name}");
                                table.Cell().Element(CellStyle).Text($"{appointment.Doctors.Users.first_name} {appointment.Doctors.Users.last_name}");
                                table.Cell().Element(CellStyle).Text(appointment.appointment_date.ToString("yyyy-MM-dd HH:mm"));
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private byte[] GenerateDoctorsPdf(List<Doctors> doctors)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Text("Doctors Report")
                        .Bold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Name");
                                header.Cell().Element(CellStyle).Text("Specialization");
                                header.Cell().Element(CellStyle).Text("Email");
                            });

                            foreach (var doctor in doctors)
                            {
                                table.Cell().Element(CellStyle).Text(doctor.doctor_id.ToString());
                                table.Cell().Element(CellStyle).Text($"{doctor.Users.first_name} {doctor.Users.last_name}");
                                table.Cell().Element(CellStyle).Text(doctor.specialization);
                                table.Cell().Element(CellStyle).Text(doctor.Users.email);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private byte[] GenerateFeedbacksPdf(List<FeedBacks> feedbacks)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Text("Feedbacks Report")
                        .Bold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Patient");
                                header.Cell().Element(CellStyle).Text("Doctor");
                                header.Cell().Element(CellStyle).Text("Rate");
                            });

                            foreach (var feedback in feedbacks)
                            {
                                table.Cell().Element(CellStyle).Text(feedback.feedback_id.ToString());
                                table.Cell().Element(CellStyle).Text($"{feedback.Patients.first_name} {feedback.Patients.last_name}");
                                table.Cell().Element(CellStyle).Text($"{feedback.Doctors.Users.first_name} {feedback.Doctors.Users.last_name}");
                                table.Cell().Element(CellStyle).Text(feedback.rate.ToString());
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private byte[] GenerateSummaryPdf(SummaryReportData summary)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text("Comprehensive Summary Report")
                        .Bold().FontSize(18).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Overview Section
                            column.Item().Text("OVERVIEW").Bold().FontSize(14);
                            column.Item().PaddingTop(0.3f, Unit.Centimetre);
                            column.Item().Text($"Total Users: {summary.TotalUsers}");
                            column.Item().Text($"Active Users: {summary.ActiveUsers}");
                            column.Item().Text($"Total Nurses: {summary.TotalNurses}");
                            column.Item().Text($"Total Doctors: {summary.TotalDoctors}");
                            column.Item().Text($"Total Patients: {summary.TotalPatients}");
                            column.Item().Text($"Total Appointments (Period): {summary.TotalAppointments}");
                            column.Item().Text($"Total Feedbacks (Period): {summary.TotalFeedbacks}");
                            column.Item().Text($"Average Rating: {summary.AverageRating}").Bold();
                            column.Item().Text($"Period: {summary.PeriodStart:yyyy-MM-dd} to {summary.PeriodEnd:yyyy-MM-dd}");
                            
                            // Top Feedbacks
                            column.Item().PaddingTop(0.5f, Unit.Centimetre);
                            column.Item().Text("TOP 5 FEEDBACKS").Bold().FontSize(14);
                            column.Item().PaddingTop(0.3f, Unit.Centimetre);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Patient");
                                    header.Cell().Element(CellStyle).Text("Doctor");
                                    header.Cell().Element(CellStyle).Text("Rate");
                                    header.Cell().Element(CellStyle).Text("Date");
                                });

                                foreach (var feedback in summary.TopFeedbacks)
                                {
                                    table.Cell().Element(CellStyle).Text(feedback.PatientName);
                                    table.Cell().Element(CellStyle).Text(feedback.DoctorName);
                                    table.Cell().Element(CellStyle).Text(feedback.Rate.ToString());
                                    table.Cell().Element(CellStyle).Text(feedback.FeedbackDate.ToString("yyyy-MM-dd"));
                                }
                            });

                            // Best Doctors
                            column.Item().PaddingTop(0.5f, Unit.Centimetre);
                            column.Item().Text("BEST 5 DOCTORS").Bold().FontSize(14);
                            column.Item().PaddingTop(0.3f, Unit.Centimetre);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Doctor");
                                    header.Cell().Element(CellStyle).Text("Specialization");
                                    header.Cell().Element(CellStyle).Text("Rating");
                                    header.Cell().Element(CellStyle).Text("Appointments");
                                });

                                foreach (var doctor in summary.BestDoctors)
                                {
                                    table.Cell().Element(CellStyle).Text(doctor.DoctorName);
                                    table.Cell().Element(CellStyle).Text(doctor.Specialization);
                                    table.Cell().Element(CellStyle).Text(Math.Round(doctor.AverageRating, 2).ToString());
                                    table.Cell().Element(CellStyle).Text(doctor.TotalAppointments.ToString());
                                }
                            });

                            // Best Nurses
                            column.Item().PaddingTop(0.5f, Unit.Centimetre);
                            column.Item().Text("BEST 5 NURSES").Bold().FontSize(14);
                            column.Item().PaddingTop(0.3f, Unit.Centimetre);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Nurse");
                                    header.Cell().Element(CellStyle).Text("Appointments");
                                });

                                foreach (var nurse in summary.BestNurses)
                                {
                                    table.Cell().Element(CellStyle).Text(nurse.NurseName);
                                    table.Cell().Element(CellStyle).Text(nurse.TotalAppointments.ToString());
                                }
                            });

                            // Most Active Patients
                            column.Item().PaddingTop(0.5f, Unit.Centimetre);
                            column.Item().Text("MOST ACTIVE 5 PATIENTS").Bold().FontSize(14);
                            column.Item().PaddingTop(0.3f, Unit.Centimetre);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Patient");
                                    header.Cell().Element(CellStyle).Text("Email");
                                    header.Cell().Element(CellStyle).Text("Appointments");
                                });

                                foreach (var patient in summary.MostPatients)
                                {
                                    table.Cell().Element(CellStyle).Text(patient.PatientName);
                                    table.Cell().Element(CellStyle).Text(patient.Email);
                                    table.Cell().Element(CellStyle).Text(patient.TotalAppointments.ToString());
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Bold();
                        });
                });
            }).GeneratePdf();
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .Padding(5)
                .Background(Colors.Grey.Lighten3);
        }
    }
}
