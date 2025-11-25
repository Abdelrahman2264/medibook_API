using medibook_API.Data;
using medibook_API.Extensions.IRepositories;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace medibook_API.Extensions.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<NotificationRepository> logger;
        public NotificationRepository(Medibook_Context database, ILogger<NotificationRepository> logger)
        {
            this.database = database;
            this.logger = logger;
        }
        public async Task<Notifications> CreateNotificationAsync(Notifications note)
        {
            try
            {
                if (note == null)
                {
                    logger.LogError("Notification is null");
                    return new Notifications();
                }
                note.create_date = DateTime.Now;
                note.is_read = false;
                await database.Notifications.AddAsync(note);
                await database.SaveChangesAsync();
                logger.LogInformation($"Notification created with ID: {note.notification_id}");
                return note;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating notification" + ex.Message);
                return new Notifications();
            }

        }
        public async Task<bool> ExistsRecentNotificationAsync(string message, int fromUserId, int toUserId, TimeSpan timeWindow)
        {
            var timeLimit = DateTime.Now.Subtract(timeWindow);
            return await database.Notifications.AnyAsync(n =>
                n.message == message &&
                n.sender_user_id == fromUserId &&
                n.reciever_user_id == toUserId &&
                n.create_date >= timeLimit);
        }


        public async Task<List<Notifications>> GetAllUnReadNotificationAsyncForUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    logger.LogError("Invalid user ID");
                    return new List<Notifications>();
                }
                var notifications = await database.Notifications
                    .Where(n => n.reciever_user_id == id && !n.is_read)
                    .OrderByDescending(n => n.create_date)
                    .ToListAsync();
                logger.LogInformation($"Retrieved {notifications.Count} unread notifications for user ID: {id}");
                return notifications;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading notification" + ex.Message);
                return new List<Notifications>();
            }
        }

        public async Task<Notifications> GetNotificationAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    logger.LogError("Invalid Notification ID");
                    return new Notifications();
                }
                var notification = await database.Notifications
                    .Include(n => n.SendUsers)
                    .Include(n => n.RecieverUsers)
                    .FirstOrDefaultAsync(n => n.notification_id == id);
                if (notification == null)
                {
                    logger.LogWarning($"Notification with ID {id} not found.");
                    return new Notifications();
                }
                return notification;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving notification" + ex.Message);
                return new Notifications();
            }

        }

        public async Task<Notifications> ReadNotificationAsync(Notifications note)
        {

            try
            {
                if (note == null)
                {
                    logger.LogError("Notification is null");
                    return new Notifications();
                }
                var existingNote = await database.Notifications.FirstOrDefaultAsync(n => n.notification_id == note.notification_id);
                if (existingNote == null)
                {
                    logger.LogWarning($"Notification with ID {note.notification_id} not found.");
                    return new Notifications();
                }
                existingNote.is_read = true;
                existingNote.read_date = DateTime.Now;
                database.Notifications.Update(existingNote);
                await database.SaveChangesAsync();
                logger.LogInformation($"Notification with ID {note.notification_id} marked as read.");
                return existingNote;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading notification" + ex.Message);
                return new Notifications();

            }
        }

        public async Task<List<Notifications>> GetAllNotificationsAsync()
        {
            try
            {
                var notifications = await database.Notifications
                    .Include(n => n.SendUsers)
                    .Include(n => n.RecieverUsers)
                    .OrderByDescending(n => n.create_date)
                    .ToListAsync();
                
                logger.LogInformation($"Retrieved {notifications.Count} notifications");
                return notifications;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all notifications: " + ex.Message);
                return new List<Notifications>();
            }
        }

        public async Task<List<Notifications>> GetNotificationsByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    logger.LogError("Invalid user ID");
                    return new List<Notifications>();
                }

                var notifications = await database.Notifications
                    .Include(n => n.SendUsers)
                    .Include(n => n.RecieverUsers)
                    .Where(n => n.reciever_user_id == userId)
                    .OrderByDescending(n => n.create_date)
                    .ToListAsync();

                logger.LogInformation($"Retrieved {notifications.Count} notifications for user ID: {userId}");
                return notifications;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving notifications for user {userId}: " + ex.Message);
                return new List<Notifications>();
            }
        }

        public async Task<List<Notifications>> GetNotificationsByUserIdAndReadStatusAsync(int userId, bool isRead)
        {
            try
            {
                if (userId <= 0)
                {
                    logger.LogError("Invalid user ID");
                    return new List<Notifications>();
                }

                var notifications = await database.Notifications
                    .Include(n => n.SendUsers)
                    .Include(n => n.RecieverUsers)
                    .Where(n => n.reciever_user_id == userId && n.is_read == isRead)
                    .OrderByDescending(n => n.create_date)
                    .ToListAsync();

                logger.LogInformation($"Retrieved {notifications.Count} {(isRead ? "read" : "unread")} notifications for user ID: {userId}");
                return notifications;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving notifications for user {userId}: " + ex.Message);
                return new List<Notifications>();
            }
        }

        public async Task<List<Notifications>> GetReadNotificationsForUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    logger.LogError("Invalid user ID");
                    return new List<Notifications>();
                }

                var notifications = await database.Notifications
                    .Include(n => n.SendUsers)
                    .Include(n => n.RecieverUsers)
                    .Where(n => n.reciever_user_id == userId && n.is_read)
                    .OrderByDescending(n => n.create_date)
                    .ToListAsync();

                logger.LogInformation($"Retrieved {notifications.Count} read notifications for user ID: {userId}");
                return notifications;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving read notifications for user {userId}: " + ex.Message);
                return new List<Notifications>();
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
        {
            try
            {
                if (notificationId <= 0)
                {
                    logger.LogError("Invalid notification ID");
                    return false;
                }

                var notification = await database.Notifications
                    .FirstOrDefaultAsync(n => n.notification_id == notificationId);

                if (notification == null)
                {
                    logger.LogWarning($"Notification with ID {notificationId} not found.");
                    return false;
                }

                if (notification.is_read)
                {
                    logger.LogInformation($"Notification {notificationId} is already marked as read.");
                    return true;
                }

                notification.is_read = true;
                notification.read_date = DateTime.Now;
                database.Notifications.Update(notification);
                await database.SaveChangesAsync();

                logger.LogInformation($"Notification {notificationId} marked as read.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error marking notification {notificationId} as read: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    logger.LogError("Invalid user ID");
                    return false;
                }

                var unreadNotifications = await database.Notifications
                    .Where(n => n.reciever_user_id == userId && !n.is_read)
                    .ToListAsync();

                if (!unreadNotifications.Any())
                {
                    logger.LogInformation($"No unread notifications found for user {userId}");
                    return true;
                }

                foreach (var notification in unreadNotifications)
                {
                    notification.is_read = true;
                    notification.read_date = DateTime.Now;
                }

                database.Notifications.UpdateRange(unreadNotifications);
                await database.SaveChangesAsync();

                logger.LogInformation($"Marked {unreadNotifications.Count} notifications as read for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error marking all notifications as read for user {userId}: " + ex.Message);
                return false;
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    logger.LogError("Invalid user ID");
                    return 0;
                }

                var count = await database.Notifications
                    .CountAsync(n => n.reciever_user_id == userId && !n.is_read);

                return count;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting unread notification count for user {userId}: " + ex.Message);
                return 0;
            }
        }
    }
}
