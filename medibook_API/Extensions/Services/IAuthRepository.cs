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

    }
    public class AuthRepository : IAuthRepository
    {
        private readonly IPasswordHasherRepository passwordHasher;
        private readonly IConfiguration configuration;
        private readonly Medibook_Context database;
        private readonly EmailServices _EmailServices;
        private readonly ILogger<AuthRepository> logger;
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
            var code = GenerateVerifyCode();
            var Code = code.ToString();
            await _EmailServices.SendVerificationEmailAsync(dto.email, dto.firstname, dto.lastname, dto.gender, Code);
            return Code;


        }

        public async Task<bool> ForgetPasswordAsync(ForgetPasswordDto dto)
        {

            try
            {
                if (dto != null)
                {
                    if (dto.newPassword != dto.confirmPassword)
                    {
                        return false;
                    }
                    var user = await database.Users.FirstOrDefaultAsync(u => u.user_id == dto.userId);
                    if (user != null)
                    {
                        user.password_hash = passwordHasher.HashPassword(dto.newPassword);
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
    }
}
