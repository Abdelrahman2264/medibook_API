using medibook_API.Extensions.IRepositories;
using medibook_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : Controller
    {
        private readonly ILogger<LogsController> _logger;
        private readonly ILogRepository _logRepository;

        public LogsController(ILogger<LogsController> logger, ILogRepository logRepository)
        {
            _logger = logger;
            _logRepository = logRepository;
        }

        // GET: /api/Logs/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<Logs>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _logRepository.GetAllLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all logs.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Logs/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Logs), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLogById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid log ID.");
                }

                var log = await _logRepository.GetLogByIdAsync(id);

                if (log == null)
                {
                    return NotFound($"Log with ID {id} not found.");
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving log with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Logs/user/{userId}
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Logs>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLogsByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var logs = await _logRepository.GetLogsByUserIdAsync(userId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving logs for user ID {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Logs/type/{logType}
        [HttpGet("type/{logType}")]
        [ProducesResponseType(typeof(IEnumerable<Logs>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLogsByType(string logType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(logType))
                {
                    return BadRequest("Log type is required.");
                }

                var logs = await _logRepository.GetLogsByTypeAsync(logType);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving logs by type {logType}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Logs/date-range?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet("date-range")]
        [ProducesResponseType(typeof(IEnumerable<Logs>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("Start date must be before or equal to end date.");
                }

                var logs = await _logRepository.GetLogsByDateRangeAsync(startDate, endDate);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving logs by date range.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Logs/current-user
        [HttpGet("current-user")]
        [ProducesResponseType(typeof(IEnumerable<Logs>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCurrentUserLogs()
        {
            try
            {
                // Get current user ID from the token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                {
                    return Unauthorized("Unable to retrieve current user information from token.");
                }

                var logs = await _logRepository.GetLogsByUserIdAsync(userId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user logs.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

