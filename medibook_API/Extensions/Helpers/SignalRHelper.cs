using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.Services;

namespace medibook_API.Extensions.Helpers
{
    /// <summary>
    /// Helper class to easily send SignalR notifications and updates from controllers
    /// </summary>
    public static class SignalRHelper
    {
        /// <summary>
        /// Send a real-time update notification when an entity is created
        /// </summary>
        public static async Task NotifyCreatedAsync(
            ISignalRService signalRService,
            string entityType,
            object entityData,
            int? userId = null)
        {
            await signalRService.SendRealTimeUpdateAsync(
                $"{entityType}_Created",
                new { EntityType = entityType, Action = "Created", Data = entityData },
                userId
            );
        }

        /// <summary>
        /// Send a real-time update notification when an entity is updated
        /// </summary>
        public static async Task NotifyUpdatedAsync(
            ISignalRService signalRService,
            string entityType,
            object entityData,
            int? userId = null)
        {
            await signalRService.SendRealTimeUpdateAsync(
                $"{entityType}_Updated",
                new { EntityType = entityType, Action = "Updated", Data = entityData },
                userId
            );
        }

        /// <summary>
        /// Send a real-time update notification when an entity is deleted
        /// </summary>
        public static async Task NotifyDeletedAsync(
            ISignalRService signalRService,
            string entityType,
            int entityId,
            int? userId = null)
        {
            await signalRService.SendRealTimeUpdateAsync(
                $"{entityType}_Deleted",
                new { EntityType = entityType, Action = "Deleted", EntityId = entityId },
                userId
            );
        }

        /// <summary>
        /// Broadcast a real-time update to all connected clients
        /// </summary>
        public static async Task BroadcastUpdateAsync(
            ISignalRService signalRService,
            string updateType,
            object data)
        {
            await signalRService.BroadcastUpdateAsync(updateType, data);
        }
    }
}

