using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace medibook_API.Extensions.Services
{
    public interface IAuthRepository
    {
        public Task<SignInResponseDto> SignInAsync(SignInDto dto);
        public Task<string> VerifyCode(VerificationRequestDto dto);
        public Task<bool> ForgetPasswordAsync(ForgetPasswordDto dto);
        public Task<object> CheckEmailAndSendCodeAsync(CheckEmailDto dto);
        public Task<object> VerifyForgetPasswordCodeAsync(VerifyForgetPasswordCodeDto dto);
        public Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
    public class AuthRepository : IAuthRepository
    {
        private readonly IPasswordHasherRepository passwordHasher;
        private readonly IConfiguration configuration;
        private readonly Medibook_Context database;
        private readonly EmailServices _EmailServices;
        private readonly ILogger<AuthRepository> logger;
        
        // In-memory storage for verification codes (email -> (code, expiration))
        private static readonly Dictionary<string, (string code, DateTime expiration)> _verificationCodes = new();
        private static readonly object _lock = new();
        
        public AuthRepository(IPasswordHasherRepository passwordHasher,
            IConfiguration configuration,
            Medibook_Context database,
            EmailServices emailServices, ILogger<AuthRepository> logger)
        {
            this.passwordHasher = passwordHasher;
            this.configuration = configuration;
            this.database = database;
            _EmailServices = emailServices;
            this.logger = logger;
        }
        public async Task<SignInResponseDto> SignInAsync(SignInDto dto)
        {
            var user = await database.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.email.ToLower() == dto.Email.ToLower());

            if (user == null || !passwordHasher.VerifyPassword(dto.Password, user.password_hash))
                return new SignInResponseDto { Message = "Invalid email or password" };

            var tokenService = new TokenService(configuration); // inject IConfiguration
            var token = tokenService.GenerateToken(user);

            return new SignInResponseDto
            {
                Token = token,
                Expiration = DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:ExpireMinutes"])),
                Message = "Login successful"
            };
        }

        public int GenerateVerifyCode()
        {
            var random = new Random();
            var Verifyocde = random.Next(100000, 1000000);
            return Verifyocde;

        }

        public async Task<string> VerifyCode(VerificationRequestDto dto)
        {

            var Code = dto.code;
            await _EmailServices.SendVerificationEmailAsync(dto.email, dto.firstname, dto.lastname, dto.gender, Code);
            return Code;


        }

        public async Task<bool> ForgetPasswordAsync(ForgetPasswordDto dto)
        {
            try
            {
                if (dto != null)
                {
                    if (dto.NewPassword != dto.ConfirmPassword)
                    {
                        return false;
                    }
                    var user = await database.Users.FirstOrDefaultAsync(u => u.user_id == dto.UserId);
                    if (user != null)
                    {
                        user.password_hash = passwordHasher.HashPassword(dto.NewPassword);
                        database.Users.Update(user);
                        await database.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ForgetPasswordAsync");
                return false;
            }
        }

        public async Task<object> CheckEmailAndSendCodeAsync(CheckEmailDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email))
                {
                    return new { success = false, message = "Email is required" };
                }

                var user = await database.Users
                    .FirstOrDefaultAsync(u => u.email.ToLower() == dto.Email.ToLower());

                if (user == null)
                {
                    return new { success = false, message = "Email not found" };
                }

                // Generate verification code
                var code = GenerateVerifyCode().ToString();
                var expiration = DateTime.Now.AddMinutes(10); // Code expires in 10 minutes

                // Store code in memory
                lock (_lock)
                {
                    _verificationCodes[dto.Email.ToLower()] = (code, expiration);
                }

                // Send verification email
                await _EmailServices.SendVerificationEmailAsync(
                    dto.Email, 
                    user.first_name, 
                    user.last_name, 
                    user.gender, 
                    code);

                logger.LogInformation("Verification code sent to {Email} for password reset", dto.Email);

                return new
                {
                    success = true,
                    message = "Verification code sent successfully",
                    email = dto.Email,
                    userId = user.user_id
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CheckEmailAndSendCodeAsync");
                return new { success = false, message = "An error occurred while processing your request" };
            }
        }

        public Task<object> VerifyForgetPasswordCodeAsync(VerifyForgetPasswordCodeDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Code))
                {
                    return Task.FromResult<object>(new { success = false, message = "Email and code are required" });
                }

                var emailKey = dto.Email.ToLower();
                bool isValid = false;

                lock (_lock)
                {
                    if (_verificationCodes.TryGetValue(emailKey, out var storedData))
                    {
                        // Check if code matches and hasn't expired
                        if (storedData.code == dto.Code && storedData.expiration > DateTime.Now)
                        {
                            isValid = true;
                            // Don't remove the code yet - we'll need it for password reset
                        }
                        else if (storedData.expiration <= DateTime.Now)
                        {
                            // Remove expired code
                            _verificationCodes.Remove(emailKey);
                        }
                    }
                }

                if (isValid)
                {
                    logger.LogInformation("Verification code verified for {Email}", dto.Email);
                    return Task.FromResult<object>(new { success = true, message = "Code verified successfully" });
                }
                else
                {
                    return Task.FromResult<object>(new { success = false, message = "Invalid or expired verification code" });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in VerifyForgetPasswordCodeAsync");
                return Task.FromResult<object>(new { success = false, message = "An error occurred while verifying the code" });
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Email) || 
                    string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.ConfirmPassword))
                {
                    return false;
                }

                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return false;
                }

                // Verify that the code was previously verified
                var emailKey = dto.Email.ToLower();
                bool codeWasVerified = false;

                lock (_lock)
                {
                    if (_verificationCodes.ContainsKey(emailKey))
                    {
                        codeWasVerified = true;
                        // Remove the code after password reset
                        _verificationCodes.Remove(emailKey);
                    }
                }

                if (!codeWasVerified)
                {
                    logger.LogWarning("Password reset attempted for {Email} without verified code", dto.Email);
                    return false;
                }

                var user = await database.Users
                    .FirstOrDefaultAsync(u => u.email.ToLower() == emailKey);

                if (user == null)
                {
                    return false;
                }

                user.password_hash = passwordHasher.HashPassword(dto.NewPassword);
                database.Users.Update(user);
                await database.SaveChangesAsync();

                logger.LogInformation("Password reset successfully for {Email}", dto.Email);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ResetPasswordAsync");
                return false;
            }
        }
    }
}
