using medibook_API.Data;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly Medibook_Context _database;
        private readonly ILogger<LogRepository> _logger;
        private readonly IUserContextService _userContextService;

        public LogRepository(
            Medibook_Context database,
            ILogger<LogRepository> logger,
            IUserContextService userContextService)
        {
            _database = database;
            _logger = logger;
            _userContextService = userContextService;
        }

        /// <summary>
        /// Creates a log entry with the current user ID from the JWT token
        /// </summary>
        public async Task<bool> CreateLogAsync(string actionType, string logType, string description)
        {
            try
            {
                var currentUserId = _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("Cannot create log: Current user ID is not available or invalid.");
                    return false;
                }

                return await CreateLogAsync(actionType, logType, description, currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating log with current user ID.");
                return false;
            }
        }

        /// <summary>
        /// Creates a log entry with a specific user ID
        /// </summary>
        public async Task<bool> CreateLogAsync(string actionType, string logType, string description, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionType) || string.IsNullOrWhiteSpace(logType) || string.IsNullOrWhiteSpace(description))
                {
                    _logger.LogWarning("Cannot create log: Required fields are missing.");
                    return false;
                }

                if (userId <= 0)
                {
                    _logger.LogWarning("Cannot create log: Invalid user ID.");
                    return false;
                }

                // Verify user exists
                var userExists = await _database.Users.AnyAsync(u => u.user_id == userId);
                if (!userExists)
                {
                    _logger.LogWarning("Cannot create log: User with ID {UserId} does not exist.", userId);
                    return false;
                }

                var log = new Logs
                {
                    action_type = actionType.Length > 50 ? actionType.Substring(0, 50) : actionType,
                    log_type = logType.Length > 50 ? logType.Substring(0, 50) : logType,
                    Description = description,
                    user_id = userId,
                    log_date = DateTime.UtcNow
                };

                await _database.Logs.AddAsync(log);
                await _database.SaveChangesAsync();

                _logger.LogInformation("Log created successfully: Action={ActionType}, Type={LogType}, UserId={UserId}", 
                    actionType, logType, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating log entry.");
                return false;
            }
        }

        /// <summary>
        /// Gets all logs for a specific user
        /// </summary>
        public async Task<IEnumerable<Logs>> GetLogsByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    _logger.LogWarning("Invalid user ID provided for GetLogsByUserIdAsync.");
                    return Enumerable.Empty<Logs>();
                }

                var logs = await _database.Logs
                    .Include(l => l.Users)
                    .Where(l => l.user_id == userId)
                    .OrderByDescending(l => l.log_date)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving logs for user ID {UserId}.", userId);
                return Enumerable.Empty<Logs>();
            }
        }

        /// <summary>
        /// Gets logs within a date range
        /// </summary>
        public async Task<IEnumerable<Logs>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    _logger.LogWarning("Invalid date range: Start date is after end date.");
                    return Enumerable.Empty<Logs>();
                }

                var logs = await _database.Logs
                    .Include(l => l.Users)
                    .Where(l => l.log_date >= startDate && l.log_date <= endDate)
                    .OrderByDescending(l => l.log_date)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving logs by date range.");
                return Enumerable.Empty<Logs>();
            }
        }

        /// <summary>
        /// Gets logs by log type
        /// </summary>
        public async Task<IEnumerable<Logs>> GetLogsByTypeAsync(string logType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(logType))
                {
                    _logger.LogWarning("Log type is required for GetLogsByTypeAsync.");
                    return Enumerable.Empty<Logs>();
                }

                var logs = await _database.Logs
                    .Include(l => l.Users)
                    .Where(l => l.log_type == logType)
                    .OrderByDescending(l => l.log_date)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving logs by type {LogType}.", logType);
                return Enumerable.Empty<Logs>();
            }
        }

        /// <summary>
        /// Gets all logs
        /// </summary>
        public async Task<IEnumerable<Logs>> GetAllLogsAsync()
        {
            try
            {
                var logs = await _database.Logs
                    .Include(l => l.Users)
                    .OrderByDescending(l => l.log_date)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all logs.");
                return Enumerable.Empty<Logs>();
            }
        }

        /// <summary>
        /// Gets a specific log by ID
        /// </summary>
        public async Task<Logs?> GetLogByIdAsync(int logId)
        {
            try
            {
                if (logId <= 0)
                {
                    _logger.LogWarning("Invalid log ID provided for GetLogByIdAsync.");
                    return null;
                }

                var log = await _database.Logs
                    .Include(l => l.Users)
                    .FirstOrDefaultAsync(l => l.log_id == logId);

                return log;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving log with ID {LogId}.", logId);
                return null;
            }
        }
    }
}
