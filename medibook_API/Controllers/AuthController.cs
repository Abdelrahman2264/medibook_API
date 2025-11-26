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
        [HttpPost("signIn")]
        [ProducesResponseType(typeof(SignInResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                {
                    logger.LogWarning("Login attempt with missing credentials.");
                    return BadRequest("Email and password are required.");
                }

                var result = await authRepository.SignInAsync(dto);

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
        [HttpPost("SignOut")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult SignOut()
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

        // NEW: Send verification code
        [HttpPost("send-verification")]
        [ProducesResponseType(typeof(VerificationRequestDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendVerificationCode([FromBody] VerificationRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.email))
                return BadRequest("Email is required.");

            var code = await authRepository.VerifyCode(dto);

            return Ok(new
            {
                Message = "Verification code sent successfully",
            });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            var result = await authRepository.ForgetPasswordAsync(dto);

            if (!result)
                return BadRequest("Failed to reset password. Make sure the passwords match and the user exists.");

            return Ok(new
            {
                Message = "Password changed successfully"
            });
        }

        // POST: /api/Auth/check-email
        [HttpPost("check-email")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CheckEmailAndSendCode([FromBody] CheckEmailDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email))
                {
                    return BadRequest("Email is required");
                }

                var result = await authRepository.CheckEmailAndSendCodeAsync(dto);
                
                // Check if result has success property
                var resultType = result.GetType();
                var successProperty = resultType.GetProperty("success");
                
                if (successProperty != null)
                {
                    var success = (bool)successProperty.GetValue(result);
                    if (!success)
                    {
                        var messageProperty = resultType.GetProperty("message");
                        var message = messageProperty?.GetValue(result)?.ToString() ?? "An error occurred";
                        return NotFound(new { message });
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking email and sending code.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Auth/verify-forget-password-code
        [HttpPost("verify-forget-password-code")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyForgetPasswordCode([FromBody] VerifyForgetPasswordCodeDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Code))
                {
                    return BadRequest("Email and code are required");
                }

                var result = await authRepository.VerifyForgetPasswordCodeAsync(dto);
                
                var resultType = result.GetType();
                var successProperty = resultType.GetProperty("success");
                
                if (successProperty != null)
                {
                    var success = (bool)successProperty.GetValue(result);
                    if (!success)
                    {
                        var messageProperty = resultType.GetProperty("message");
                        var message = messageProperty?.GetValue(result)?.ToString() ?? "Invalid code";
                        return BadRequest(new { message });
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while verifying forget password code.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Auth/reset-password
        [HttpPost("reset-password")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid request");

                if (string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.ConfirmPassword))
                    return BadRequest("New password and confirm password are required");

                if (dto.NewPassword != dto.ConfirmPassword)
                    return BadRequest("Passwords do not match");

                var result = await authRepository.ResetPasswordAsync(dto);

                if (!result)
                    return BadRequest("Failed to reset password. Please verify your email and code first.");

                return Ok(new
                {
                    Message = "Password reset successfully"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while resetting password.");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
