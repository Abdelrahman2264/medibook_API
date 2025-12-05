using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int senderUserId, int receiverUserId, string message);
        Task SendNotificationsToUsersAsync(int senderUserId, List<int> receiverUserIds, string message);
        Task SendNotificationToAdminsAsync(int senderUserId, string message);
        Task SendNotificationToDoctorAsync(int senderUserId, int doctorUserId, string message);
        Task SendNotificationToPatientAsync(int senderUserId, int patientUserId, string message);
        Task SendNotificationToNurseAsync(int senderUserId, int nurseUserId, string message);
        Task<string> FormatUserNameWithTitleAsync(Users user);
        string FormatUserNameWithTitle(Users user, bool isDoctor = false);
    }

    public class NotificationService : INotificationService
    {
        private readonly Medibook_Context _database;
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationRepository _notificationRepository;
        private readonly ISignalRService _signalRService;

        public NotificationService(
            Medibook_Context database,
            ILogger<NotificationService> logger,
            INotificationRepository notificationRepository,
            ISignalRService signalRService)
        {
            _database = database;
            _logger = logger;
            _notificationRepository = notificationRepository;
            _signalRService = signalRService;
        }

        public async Task<string> FormatUserNameWithTitleAsync(Users user)
        {
            if (user == null)
                return "User";

            // Check if user is a doctor
            var isDoctor = user.Doctors != null ;

            if (!isDoctor)
            {
                isDoctor = await _database.Doctors.AnyAsync(d => d.user_id == user.user_id);
            }

            return FormatUserNameWithTitle(user, isDoctor);
        }

        public string FormatUserNameWithTitle(Users user, bool isDoctor = false)
        {
            if (user == null)
                return "User";

            if (isDoctor)
            {
                return $"Dr. {user.first_name} {user.last_name}";
            }

            // For other users, use Mr./Mrs. based on gender
            var title = user.gender?.ToLower() == "male" ? "Mr." : "Mrs.";
            return $"{title} {user.first_name} {user.last_name}";
        }

        public async Task SendNotificationAsync(int senderUserId, int receiverUserId, string message)
        {
            try
            {
                // Get sender and receiver to format names
                var sender = await _database.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.user_id == senderUserId);

                var receiver = await _database.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.user_id == receiverUserId);

                if (sender == null || receiver == null)
                {
                    _logger.LogWarning($"Cannot send notification: Sender {senderUserId} or Receiver {receiverUserId} not found.");
                    return;
                }

                // Check if sender and receiver are doctors
                var senderIsDoctor = await _database.Doctors.AnyAsync(d => d.user_id == senderUserId);
                var receiverIsDoctor = await _database.Doctors.AnyAsync(d => d.user_id == receiverUserId);

                var senderName = FormatUserNameWithTitle(sender, senderIsDoctor);
                var receiverName = FormatUserNameWithTitle(receiver, receiverIsDoctor);

                // Format message with names
                var formattedMessage = message
                    .Replace("{SenderName}", senderName)
                    .Replace("{ReceiverName}", receiverName);

                var notification = new Notifications
                {
                    sender_user_id = senderUserId,
                    reciever_user_id = receiverUserId,
                    message = formattedMessage,
                    is_read = false,
                    create_date = DateTime.Now,
                    read_date = DateTime.Now
                };

                var createdNotification = await _notificationRepository.CreateNotificationAsync(notification);
                _logger.LogInformation($"Notification sent from {senderUserId} to {receiverUserId}");

                // Send real-time notification via SignalR immediately after creation
                if (createdNotification.notification_id > 0)
                {
                    try
                    {
                        var notificationDto = await MapToNotificationDetailsDtoAsync(createdNotification);
                        await _signalRService.SendNotificationToUserAsync(receiverUserId, notificationDto);
                        _logger.LogInformation($"SignalR notification sent to user {receiverUserId} for notification {createdNotification.notification_id}");
                    }
                    catch (Exception signalREx)
                    {
                        _logger.LogError(signalREx, $"Failed to send SignalR notification to user {receiverUserId}, but notification was saved to database");
                        // Don't throw - notification is already saved, SignalR failure shouldn't break the flow
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification from {senderUserId} to {receiverUserId}: {ex.Message}");
            }
        }

        private async Task<NotificationDetailsDto> MapToNotificationDetailsDtoAsync(Notifications notification)
        {
            // Ensure related entities are loaded
            if (notification.SendUsers == null || notification.RecieverUsers == null)
            {
                notification = await _database.Notifications
                    .Include(n => n.SendUsers)
                    .Include(n => n.RecieverUsers)
                    .FirstOrDefaultAsync(n => n.notification_id == notification.notification_id);
            }

            return new NotificationDetailsDto
            {
                NotificationId = notification.notification_id,
                Message = notification.message,
                IsRead = notification.is_read,
                CreateDate = notification.create_date,
                ReadDate = notification.is_read ? notification.read_date : null,
                SenderUserId = notification.sender_user_id,
                SenderName = notification.SendUsers != null
                    ? $"{notification.SendUsers.first_name} {notification.SendUsers.last_name}"
                    : "Unknown",
                SenderEmail = notification.SendUsers?.email ?? "N/A",
                ReceiverUserId = notification.reciever_user_id,
                ReceiverName = notification.RecieverUsers != null
                    ? $"{notification.RecieverUsers.first_name} {notification.RecieverUsers.last_name}"
                    : "Unknown",
                ReceiverEmail = notification.RecieverUsers?.email ?? "N/A"
            };
        }

        public async Task SendNotificationsToUsersAsync(int senderUserId, List<int> receiverUserIds, string message)
        {
            try
            {
                var tasks = receiverUserIds.Select(userId => SendNotificationAsync(senderUserId, userId, message));
                await Task.WhenAll(tasks);
                _logger.LogInformation($"Sent notifications to {receiverUserIds.Count} users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notifications to multiple users: {ex.Message}");
            }
        }

        public async Task SendNotificationToAdminsAsync(int senderUserId, string message)
        {
            try
            {
                // Get all admin users (assuming role_name contains "Admin" or "admin")
                var adminUsers = await _database.Users
                    .Include(u => u.Role)
                    .Where(u => u.is_active &&
                                (u.Role.role_name.ToLower() == "admin" ||
                                 u.Role.role_name.ToLower() == "administrator")
                                 )
                    .Select(u => u.user_id)
                    .ToListAsync();

                if (adminUsers.Any())
                {
                    await SendNotificationsToUsersAsync(senderUserId, adminUsers, message);
                    _logger.LogInformation($"Sent notification to {adminUsers.Count} admin users");
                }
                else
                {
                    _logger.LogWarning("No admin users found to send notification");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to admins: {ex.Message}");
            }
        }

        public async Task SendNotificationToDoctorAsync(int senderUserId, int doctorUserId, string message)
        {
            try
            {
                // Verify user is a doctor
                var isDoctor = await _database.Doctors
                    .AnyAsync(d => d.user_id == doctorUserId);

                if (!isDoctor)
                {
                    _logger.LogWarning($"User {doctorUserId} is not a doctor");
                    return;
                }

                await SendNotificationAsync(senderUserId, doctorUserId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to doctor {doctorUserId}: {ex.Message}");
            }
        }

        public async Task SendNotificationToPatientAsync(int senderUserId, int patientUserId, string message)
        {
            try
            {
                await SendNotificationAsync(senderUserId, patientUserId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to patient {patientUserId}: {ex.Message}");
            }
        }

        public async Task SendNotificationToNurseAsync(int senderUserId, int nurseUserId, string message)
        {
            try
            {
                // Verify user is a nurse
                var isNurse = await _database.Nurses
                    .AnyAsync(n => n.user_id == nurseUserId);

                if (!isNurse)
                {
                    _logger.LogWarning($"User {nurseUserId} is not a nurse");
                    return;
                }

                await SendNotificationAsync(senderUserId, nurseUserId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to nurse {nurseUserId}: {ex.Message}");
            }
        }
    }
}

