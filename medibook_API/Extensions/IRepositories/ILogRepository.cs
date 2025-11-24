using medibook_API.Models;

namespace medibook_API.Extensions.IRepositories
{
    public interface ILogRepository
    {
        Task<bool> CreateLogAsync(string actionType, string logType, string description);
        Task<bool> CreateLogAsync(string actionType, string logType, string description, int userId);
        Task<IEnumerable<Logs>> GetLogsByUserIdAsync(int userId);
        Task<IEnumerable<Logs>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Logs>> GetLogsByTypeAsync(string logType);
        Task<IEnumerable<Logs>> GetAllLogsAsync();
        Task<Logs?> GetLogByIdAsync(int logId);
    }
}
