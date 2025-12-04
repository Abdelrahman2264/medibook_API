using medibook_API.Extensions.DTOs;
using medibook_API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace medibook_API.Extensions.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<SignalRService> _logger;

        public SignalRService(
            IHubContext<NotificationHub> hubContext,
            ILogger<SignalRService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendNotificationToUserAsync(int userId, NotificationDetailsDto notification)
        {
            try
            {
                await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification);
                _logger.LogInformation($"SignalR notification sent to user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending SignalR notification to user {userId}: {ex.Message}");
            }
        }

        public async Task SendNotificationToUsersAsync(List<int> userIds, NotificationDetailsDto notification)
        {
            try
            {
                var tasks = userIds.Select(userId => 
                    _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification));
                await Task.WhenAll(tasks);
                _logger.LogInformation($"SignalR notification sent to {userIds.Count} users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending SignalR notifications to multiple users: {ex.Message}");
            }
        }

        public async Task SendRealTimeUpdateAsync(string updateType, object data, int? userId = null)
        {
            try
            {
                if (userId.HasValue)
                {
                    await _hubContext.Clients.Group($"User_{userId.Value}").SendAsync("ReceiveUpdate", new
                    {
                        Type = updateType,
                        Data = data,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", new
                    {
                        Type = updateType,
                        Data = data,
                        Timestamp = DateTime.UtcNow
                    });
                }
                _logger.LogInformation($"SignalR update sent: {updateType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending SignalR update {updateType}: {ex.Message}");
            }
        }

        public async Task BroadcastUpdateAsync(string updateType, object data)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", new
                {
                    Type = updateType,
                    Data = data,
                    Timestamp = DateTime.UtcNow
                });
                _logger.LogInformation($"SignalR broadcast sent: {updateType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error broadcasting SignalR update {updateType}: {ex.Message}");
            }
        }
    }
}






