using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserContextService _userContextService;

        public NotificationsController(
            ILogger<NotificationsController> logger,
            INotificationRepository notificationRepository,
            INotificationService notificationService,
            IUserContextService userContextService)
        {
            _logger = logger;
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
            _userContextService = userContextService;
        }

        // GET: /api/Notifications/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var notifications = await _notificationRepository.GetAllNotificationsAsync();
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all notifications.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/user/{userId}
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationsByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving notifications for user {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/current-user
        [HttpGet("current-user")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCurrentUserNotifications()
        {
            try
            {
                var currentUserId = _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                {
                    return Unauthorized("Unable to retrieve current user information from token.");
                }

                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(currentUserId);
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user notifications.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/user/{userId}/unread
        [HttpGet("user/{userId:int}/unread")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUnreadNotificationsByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var notifications = await _notificationRepository.GetNotificationsByUserIdAndReadStatusAsync(userId, false);
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving unread notifications for user {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/current-user/unread
        [HttpGet("current-user/unread")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCurrentUserUnreadNotifications()
        {
            try
            {
                var currentUserId = _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                {
                    return Unauthorized("Unable to retrieve current user information from token.");
                }

                var notifications = await _notificationRepository.GetNotificationsByUserIdAndReadStatusAsync(currentUserId, false);
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user unread notifications.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/user/{userId}/read
        [HttpGet("user/{userId:int}/read")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReadNotificationsByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var notifications = await _notificationRepository.GetReadNotificationsForUserAsync(userId);
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving read notifications for user {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/user/{userId}/status/{isRead}
        [HttpGet("user/{userId:int}/status/{isRead:bool}")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationsByUserIdAndStatus(int userId, bool isRead)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var notifications = await _notificationRepository.GetNotificationsByUserIdAndReadStatusAsync(userId, isRead);
                var notificationDtos = notifications.Select(n => MapToNotificationDetailsDto(n)).ToList();
                return Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving notifications for user {userId} with status {isRead}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(NotificationDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid notification ID.");
                }

                var notification = await _notificationRepository.GetNotificationAsync(id);

                if (notification == null || notification.notification_id == 0)
                {
                    return NotFound($"Notification with ID {id} not found.");
                }

                var notificationDto = MapToNotificationDetailsDto(notification);
                return Ok(notificationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving notification {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/current-user/unread-count
        [HttpGet("current-user/unread-count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            try
            {
                var currentUserId = _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                {
                    return Unauthorized("Unable to retrieve current user information from token.");
                }

                var count = await _notificationRepository.GetUnreadNotificationCountAsync(currentUserId);
                return Ok(new { UnreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unread notification count.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Notifications/user/{userId}/unread-count
        [HttpGet("user/{userId:int}/unread-count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUnreadNotificationCountByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var count = await _notificationRepository.GetUnreadNotificationCountAsync(userId);
                return Ok(new { UnreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving unread notification count for user {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PATCH: /api/Notifications/{id}/mark-read
        [HttpPatch("{id:int}/mark-read")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            try
            {
            
                if (id <= 0)
                {
                    return BadRequest("Invalid notification ID.");
                }

                var result = await _notificationRepository.MarkNotificationAsReadAsync(id);

                if (!result)
                {
                    return NotFound($"Notification with ID {id} not found.");
                }

                return Ok(new { Message = "Notification marked as read successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while marking notification {id} as read.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PATCH: /api/Notifications/current-user/mark-all-read
        [HttpPatch("current-user/mark-all-read")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            try
            {
                var currentUserId = _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                {
                    return Unauthorized("Unable to retrieve current user information from token.");
                }

                var result = await _notificationRepository.MarkAllNotificationsAsReadAsync(currentUserId);

                if (!result)
                {
                    return StatusCode(500, "Failed to mark all notifications as read.");
                }

                return Ok(new { Message = "All notifications marked as read successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking all notifications as read.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PATCH: /api/Notifications/user/{userId}/mark-all-read
        [HttpPatch("user/{userId:int}/mark-all-read")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MarkAllNotificationsAsReadByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var result = await _notificationRepository.MarkAllNotificationsAsReadAsync(userId);

                if (!result)
                {
                    return StatusCode(500, "Failed to mark all notifications as read.");
                }

                return Ok(new { Message = "All notifications marked as read successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while marking all notifications as read for user {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        private NotificationDetailsDto MapToNotificationDetailsDto(Notifications notification)
        {
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
    }
}








