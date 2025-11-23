using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> logger;
        private readonly IAuthRepository authRepository;

        public AuthController(ILogger<AuthController> logger, IAuthRepository authRepository)
        {
            this.logger = logger;
            this.authRepository = authRepository;
        }

        // POST: /api/Auth/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                {
                    logger.LogWarning("Login attempt with missing credentials.");
                    return BadRequest("Email and password are required.");
                }

                var result = await authRepository.LoginAsync(dto);

                if (string.IsNullOrEmpty(result.Token))
                {
                    logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                    return BadRequest(result.Message);
                }

                logger.LogInformation("User {Email} logged in successfully.", dto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while logging in user {Email}.", dto?.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Auth/logout
        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Logout()
        {
            try
            {
                // JWT is stateless; logout is usually handled client-side
                logger.LogInformation("User logged out.");
                return Ok(new { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during logout.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
