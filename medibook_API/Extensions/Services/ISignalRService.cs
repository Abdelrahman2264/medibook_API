using medibook_API.Extensions.DTOs;

namespace medibook_API.Extensions.Services
{
    public interface ISignalRService
    {
        Task SendNotificationToUserAsync(int userId, NotificationDetailsDto notification);
        Task SendNotificationToUsersAsync(List<int> userIds, NotificationDetailsDto notification);
        Task SendRealTimeUpdateAsync(string updateType, object data, int? userId = null);
        Task BroadcastUpdateAsync(string updateType, object data);
    }
}





