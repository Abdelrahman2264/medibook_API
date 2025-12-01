using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.DTOs.medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class FeedBackRepository : IFeedBackRepository
    {
        private readonly Medibook_Context _database;
        private readonly ILogger<FeedBackRepository> _logger;
        private readonly ILogRepository _logRepository;
        private readonly IUserContextService _userContextService;
        private readonly INotificationService _notificationService;

        public FeedBackRepository(
            Medibook_Context database,
            ILogger<FeedBackRepository> logger,
            ILogRepository logRepository,
            IUserContextService userContextService,
            INotificationService notificationService)
        {
            _database = database;
            _logger = logger;
            _logRepository = logRepository;
            _userContextService = userContextService;
            _notificationService = notificationService;
        }

        public async Task<FeedbackResponseDto> CreateFeedbackAsync(CreateFeedbackDto dto)
        {
            try
            {
                // Validate appointment exists and belongs to patient and doctor
                var appointment = await _database.Appointments
                    .FirstOrDefaultAsync(a => a.appointment_id == dto.AppointmentId &&
                                             a.patient_id == dto.PatientId &&
                                             a.doctor_id == dto.DoctorId);

                if (appointment == null)
                {
                    _logger.LogWarning($"Appointment {dto.AppointmentId} not found or doesn't match patient/doctor.");
                    await _logRepository.CreateLogAsync("CREATE_FEEDBACK", "WARNING",
                        $"Failed to create feedback: Appointment {dto.AppointmentId} not found or doesn't match Patient {dto.PatientId} and Doctor {dto.DoctorId}");

                    return new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Appointment not found or doesn't match the provided patient and doctor.",
                        Success = false
                    };
                }

                // Check if feedback already exists for this appointment
                var existingFeedback = await _database.FeedBacks
                    .FirstOrDefaultAsync(f => f.appointment_id == dto.AppointmentId);

                if (existingFeedback != null)
                {
                    _logger.LogWarning($"Feedback already exists for appointment {dto.AppointmentId}.");
                    await _logRepository.CreateLogAsync("CREATE_FEEDBACK", "WARNING",
                        $"Failed to create feedback: Feedback already exists for appointment {dto.AppointmentId}");

                    return new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Feedback already exists for this appointment.",
                        Success = false
                    };
                }

                var feedback = new FeedBacks
                {
                    patient_id = dto.PatientId,
                    doctor_id = dto.DoctorId,
                    appointment_id = dto.AppointmentId,
                    comment = dto.Comment,
                    rate = dto.Rate,
                    feedback_date = DateTime.Now,
                    doctor_reply = string.Empty,
                    reply_date = null,
                    is_favourite = false
                };

                await _database.FeedBacks.AddAsync(feedback);
                await _database.SaveChangesAsync();

                var currentUserId = _userContextService.GetCurrentUserId();
                await _logRepository.CreateLogAsync("CREATE_FEEDBACK", "SUCCESS",
                    $"Feedback created successfully. Feedback ID: {feedback.feedback_id}, Patient ID: {dto.PatientId}, Doctor ID: {dto.DoctorId}, Appointment ID: {dto.AppointmentId}, Rate: {dto.Rate}");

                // Send notifications
                var senderId = currentUserId > 0 ? currentUserId : dto.PatientId;

                // Get doctor user ID
                var doctor = await _database.Doctors
                    .FirstOrDefaultAsync(d => d.doctor_id == dto.DoctorId);

                if (doctor != null)
                {
                    var doctorMessage = $"You have received new feedback (Rating: {dto.Rate}/5) for appointment {dto.AppointmentId}.";
                    await _notificationService.SendNotificationToDoctorAsync(senderId, doctor.user_id, doctorMessage);
                }

                // Send to all admins
                var adminMessage = $"New feedback created. Feedback ID: {feedback.feedback_id}, Patient ID: {dto.PatientId}, Doctor ID: {dto.DoctorId}, Rate: {dto.Rate}/5";
                await _notificationService.SendNotificationToAdminsAsync(senderId, adminMessage);

                return new FeedbackResponseDto
                {
                    FeedbackId = feedback.feedback_id,
                    Message = "Feedback created successfully.",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating feedback: {ex.Message}");
                await _logRepository.CreateLogAsync("CREATE_FEEDBACK", "ERROR",
                    $"Error creating feedback: {ex.Message}. Patient ID: {dto.PatientId}, Doctor ID: {dto.DoctorId}");

                return new FeedbackResponseDto
                {
                    FeedbackId = 0,
                    Message = "An error occurred while creating feedback.",
                    Success = false
                };
            }
        }

        public async Task<FeedbackResponseDto> AddDoctorReplyAsync(DoctorReplyDto dto)
        {
            try
            {
                var feedback = await _database.FeedBacks
                    .FirstOrDefaultAsync(f => f.feedback_id == dto.FeedbackId);

                if (feedback == null)
                {
                    _logger.LogWarning($"Feedback {dto.FeedbackId} not found for adding doctor reply.");
                    await _logRepository.CreateLogAsync("ADD_DOCTOR_REPLY", "WARNING",
                        $"Failed to add doctor reply: Feedback {dto.FeedbackId} not found");

                    return new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Feedback not found.",
                        Success = false
                    };
                }

                feedback.doctor_reply = dto.DoctorReply;
                feedback.reply_date = DateTime.Now;

                _database.FeedBacks.Update(feedback);
                await _database.SaveChangesAsync();

                var currentUserId = _userContextService.GetCurrentUserId();
                await _logRepository.CreateLogAsync("ADD_DOCTOR_REPLY", "SUCCESS",
                    $"Doctor reply added to feedback {dto.FeedbackId}. Doctor ID: {feedback.doctor_id}");

                // Send notifications
                var senderId = currentUserId > 0 ? currentUserId : feedback.doctor_id;

                // Notify patient
                var patientMessage = $"Dr. {senderId} has replied to your feedback (ID: {dto.FeedbackId}).";
                await _notificationService.SendNotificationToPatientAsync(senderId, feedback.patient_id, patientMessage);

                // Send to all admins
                var adminMessage = $"Doctor reply added to feedback {dto.FeedbackId}. Doctor ID: {feedback.doctor_id}, Patient ID: {feedback.patient_id}";
                await _notificationService.SendNotificationToAdminsAsync(senderId, adminMessage);

                return new FeedbackResponseDto
                {
                    FeedbackId = feedback.feedback_id,
                    Message = "Doctor reply added successfully.",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding doctor reply: {ex.Message}");
                await _logRepository.CreateLogAsync("ADD_DOCTOR_REPLY", "ERROR",
                    $"Error adding doctor reply to feedback {dto.FeedbackId}: {ex.Message}");

                return new FeedbackResponseDto
                {
                    FeedbackId = 0,
                    Message = "An error occurred while adding doctor reply.",
                    Success = false
                };
            }
        }

        public async Task<bool> DeleteFeedbackAsync(int feedbackId)
        {
            try
            {
                var feedback = await _database.FeedBacks
                    .FirstOrDefaultAsync(f => f.feedback_id == feedbackId);

                if (feedback == null)
                {
                    _logger.LogWarning($"Feedback {feedbackId} not found for deletion.");
                    await _logRepository.CreateLogAsync("DELETE_FEEDBACK", "WARNING",
                        $"Failed to delete feedback: Feedback {feedbackId} not found");
                    return false;
                }

                _database.FeedBacks.Remove(feedback);
                await _database.SaveChangesAsync();

                var currentUserId = _userContextService.GetCurrentUserId();
                await _logRepository.CreateLogAsync("DELETE_FEEDBACK", "SUCCESS",
                    $"Feedback {feedbackId} deleted successfully. Patient ID: {feedback.patient_id}, Doctor ID: {feedback.doctor_id}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting feedback {feedbackId}: {ex.Message}");
                await _logRepository.CreateLogAsync("DELETE_FEEDBACK", "ERROR",
                    $"Error deleting feedback {feedbackId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ToggleFavouriteAsync(int feedbackId)
        {
            try
            {
                var feedback = await _database.FeedBacks
                    .FirstOrDefaultAsync(f => f.feedback_id == feedbackId);

                if (feedback == null)
                {
                    _logger.LogWarning($"Feedback {feedbackId} not found for toggling favourite.");
                    await _logRepository.CreateLogAsync("TOGGLE_FAVOURITE_FEEDBACK", "WARNING",
                        $"Failed to toggle favourite: Feedback {feedbackId} not found");
                    return false;
                }

                feedback.is_favourite = !feedback.is_favourite;
                _database.FeedBacks.Update(feedback);
                await _database.SaveChangesAsync();

                var currentUserId = _userContextService.GetCurrentUserId();
                await _logRepository.CreateLogAsync("TOGGLE_FAVOURITE_FEEDBACK", "SUCCESS",
                    $"Feedback {feedbackId} favourite status toggled to {feedback.is_favourite}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling favourite for feedback {feedbackId}: {ex.Message}");
                await _logRepository.CreateLogAsync("TOGGLE_FAVOURITE_FEEDBACK", "ERROR",
                    $"Error toggling favourite for feedback {feedbackId}: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<FeedbackDetailsDto>> GetAllFeedbacksAsync()
        {
            try
            {
                var feedbacks = await _database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                        .ThenInclude(d => d.Users)
                    .Include(f => f.Appointments)
                    .AsNoTracking()
                    .OrderByDescending(f => f.feedback_date)
                    .ToListAsync();

                return feedbacks.Select(f => MapToFeedbackDetailsDto(f));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all feedbacks: {ex.Message}");
                return Enumerable.Empty<FeedbackDetailsDto>();
            }
        }

        public async Task<IEnumerable<FeedbackDetailsDto>> GetFeedbacksByDoctorIdAsync(int doctorId)
        {
            try
            {
                var feedbacks = await _database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                        .ThenInclude(d => d.Users)
                    .Include(f => f.Appointments)
                    .Where(f => f.doctor_id == doctorId)
                    .AsNoTracking()
                    .OrderByDescending(f => f.feedback_date)
                    .ToListAsync();

                return feedbacks.Select(f => MapToFeedbackDetailsDto(f));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving feedbacks for doctor {doctorId}: {ex.Message}");
                return Enumerable.Empty<FeedbackDetailsDto>();
            }
        }

        public async Task<IEnumerable<FeedbackDetailsDto>> GetFeedbacksByPatientIdAsync(int patientId)
        {
            try
            {
                var feedbacks = await _database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                        .ThenInclude(d => d.Users)
                    .Include(f => f.Appointments)
                    .Where(f => f.patient_id == patientId)
                    .AsNoTracking()
                    .OrderByDescending(f => f.feedback_date)
                    .ToListAsync();

                return feedbacks.Select(f => MapToFeedbackDetailsDto(f));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving feedbacks for patient {patientId}: {ex.Message}");
                return Enumerable.Empty<FeedbackDetailsDto>();
            }
        }

        public async Task<IEnumerable<FeedbackDetailsDto>> GetFeedbacksByNurseIdAsync(int nurseId)
        {
            try
            {
                // Get feedbacks for appointments where the nurse was assigned
                var feedbacks = await _database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                        .ThenInclude(d => d.Users)
                    .Include(f => f.Appointments)
                    .Where(f => f.Appointments != null && f.Appointments.nurse_id == nurseId)
                    .AsNoTracking()
                    .OrderByDescending(f => f.feedback_date)
                    .ToListAsync();

                return feedbacks.Select(f => MapToFeedbackDetailsDto(f));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving feedbacks for nurse {nurseId}: {ex.Message}");
                return Enumerable.Empty<FeedbackDetailsDto>();
            }
        }

        public async Task<FeedbackDetailsDto?> GetFeedbackByIdAsync(int feedbackId)
        {
            try
            {
                var feedback = await _database.FeedBacks
                    .Include(f => f.Patients)
                    .Include(f => f.Doctors)
                        .ThenInclude(d => d.Users)
                    .Include(f => f.Appointments)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.feedback_id == feedbackId);

                if (feedback == null)
                {
                    return null;
                }

                return MapToFeedbackDetailsDto(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving feedback {feedbackId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateFeedbackAsync(UpdateFeedbackDto dto)
        {
            try
            {
                var feedback = await _database.FeedBacks
                    .FirstOrDefaultAsync(f => f.feedback_id == dto.FeedbackId);

                if (feedback == null)
                {
                    _logger.LogWarning($"Feedback {dto.FeedbackId} not found for update.");
                    await _logRepository.CreateLogAsync("UPDATE_FEEDBACK", "WARNING",
                        $"Failed to update feedback: Feedback {dto.FeedbackId} not found");
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    feedback.comment = dto.Comment;
                }


                _database.FeedBacks.Update(feedback);
                await _database.SaveChangesAsync();

                var currentUserId = _userContextService.GetCurrentUserId();
                await _logRepository.CreateLogAsync("UPDATE_FEEDBACK", "SUCCESS",
                    $"Feedback {dto.FeedbackId} updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating feedback {dto.FeedbackId}: {ex.Message}");
                await _logRepository.CreateLogAsync("UPDATE_FEEDBACK", "ERROR",
                    $"Error updating feedback {dto.FeedbackId}: {ex.Message}");
                return false;
            }
        }

        private FeedbackDetailsDto MapToFeedbackDetailsDto(FeedBacks feedback)
        {
            return new FeedbackDetailsDto
            {
                FeedbackId = feedback.feedback_id,
                PatientId = feedback.patient_id,
                PatientFirstName = feedback.Patients?.first_name ?? "N/A",
                PatientLastName = feedback.Patients?.last_name ?? "N/A",
                PatientEmail = feedback.Patients?.email ?? "N/A",
                DoctorId = feedback.doctor_id,
                DoctorFirstName = feedback.Doctors?.Users?.first_name ?? "N/A",
                DoctorLastName = feedback.Doctors?.Users?.last_name ?? "N/A",
                DoctorSpecialization = feedback.Doctors?.specialization ?? "N/A",
                AppointmentId = feedback.appointment_id,
                AppointmentDate = feedback.Appointments?.appointment_date ?? DateTime.MinValue,
                Comment = feedback.comment,
                Rate = feedback.rate,
                FeedbackDate = feedback.feedback_date,
                DoctorReply = feedback.doctor_reply ?? string.Empty,
                ReplyDate = feedback.reply_date,
                IsFavourite = feedback.is_favourite
            };
        }
    }
}
