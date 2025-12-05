using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly EmailServices _emailService;
        private readonly IConfiguration _configuration;

        public ContactController(
            ILogger<ContactController> logger,
            EmailServices emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        // POST: /api/Contact/send
        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendContactMessage([FromBody] ContactUsDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { success = false, message = "Validation failed.", errors });
                }

                // Get team email from configuration, default to the email service email if not configured
                var teamEmail = _configuration["ContactUs:TeamEmail"] 
                    ?? _configuration["EmailSettings:Email"] 
                    ?? "info@medibook.com";

                // Send email to team
                var emailSent = await _emailService.SendContactUsEmailAsync(
                    teamEmail,
                    dto.Name,
                    dto.Email,
                    dto.Phone,
                    dto.Subject,
                    dto.Message
                );

                if (!emailSent)
                {
                    _logger.LogError("Failed to send contact us email");
                    return StatusCode((int)HttpStatusCode.InternalServerError, 
                        new { success = false, message = "Failed to send email. Please try again later." });
                }

                _logger.LogInformation($"Contact us message received from {dto.Email} - Subject: {dto.Subject}");

                return Ok(new { success = true, message = "Your message has been sent successfully. We'll get back to you soon!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact us request");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { success = false, message = "An error occurred while processing your request." });
            }
        }
    }
}

