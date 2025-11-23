using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace medibook_API.Extensions.Services
{
    public interface IAuthRepository
    {
        public Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
    public class AuthRepository : IAuthRepository
    {
        private readonly IPasswordHasherRepository passwordHasher;
        private readonly IConfiguration configuration;
        private readonly Medibook_Context database;
        public AuthRepository(IPasswordHasherRepository passwordHasher,
            IConfiguration configuration,
            Medibook_Context database)
        {
            this.passwordHasher = passwordHasher;
            this.configuration = configuration;
            this.database = database;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await database.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.email.ToLower() == dto.Email.ToLower());

            if (user == null || !passwordHasher.VerifyPassword(dto.Password, user.password_hash))
                return new LoginResponseDto { Message = "Invalid email or password" };

            var tokenService = new TokenService(configuration); // inject IConfiguration
            var token = tokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:ExpireMinutes"])),
                Message = "Login successful"
            };
        }

    }
}
