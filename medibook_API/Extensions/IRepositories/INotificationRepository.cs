using medibook_API.Models;

namespace medibook_API.Extensions.IRepositories
{
    public interface INotificationRepository
    {
        public Task<Notifications> CreateNotificationAsync(Notifications note);
        public Task<Notifications> ReadNotificationAsync(Notifications note);
        public Task<Notifications> GetNotificationAsync(int id);
        public Task<List<Notifications>> GetAllUnReadNotificationAsyncForUser(int id);
        public Task<bool> ExistsRecentNotificationAsync(string message, int fromUserId, int toUserId, TimeSpan timeWindow);
        public Task<List<Notifications>> GetAllNotificationsAsync();
        public Task<List<Notifications>> GetNotificationsByUserIdAsync(int userId);
        public Task<List<Notifications>> GetNotificationsByUserIdAndReadStatusAsync(int userId, bool isRead);
        public Task<List<Notifications>> GetReadNotificationsForUserAsync(int userId);
        public Task<bool> MarkNotificationAsReadAsync(int notificationId);
        public Task<bool> MarkAllNotificationsAsReadAsync(int userId);
        public Task<int> GetUnreadNotificationCountAsync(int userId);
    }
}
