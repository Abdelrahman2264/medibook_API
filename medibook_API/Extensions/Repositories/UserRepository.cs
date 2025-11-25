using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<UserRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly IPasswordHasherRepository passwordHasher;
        private readonly ILogRepository logRepository;

        public UserRepository(
            Medibook_Context database,
            ILogger<UserRepository> logger,
            StringNormalizer stringNormalizer,
            IPasswordHasherRepository passwordHasher,
            ILogRepository logRepository)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
            this.passwordHasher = passwordHasher;
            this.logRepository = logRepository;
        }
        public async Task<bool> ActiveUserAsync(int id)
        {
            try
            {
                var user = await database.Users.FirstOrDefaultAsync(r => r.user_id == id);

                if (user == null)
                {
                    logger.LogWarning("ActiveUserAsync: User {UserId} not found.", id);
                    await logRepository.CreateLogAsync("Activate User", "Error", $"User {id} not found.");
                    return false;
                }

                user.is_active = true;
                await database.SaveChangesAsync();

                logger.LogInformation("ActiveUserAsync: User {UserId} activated successfully.", id);
                await logRepository.CreateLogAsync("Activate User", "Success", $"User {id} activated.");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActiveUserAsync error");
                await logRepository.CreateLogAsync("Activate User", "Error", ex.Message);
                return false;
            }
        }

        public async Task<CreatedResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                var role = await database.Roles.FirstOrDefaultAsync(u => u.role_name == "user");

                if (role == null)
                {
                    logger.LogWarning("User role not found");
                    await logRepository.CreateLogAsync("Create User", "Error", "User role not found");

                    return new CreatedResponseDto { Message = "User role not found" };
                }

                if (dto == null)
                {
                    logger.LogWarning("Invalid user data");
                    await logRepository.CreateLogAsync("Create User", "Error", "Invalid user data");

                    return new CreatedResponseDto { Message = "Invalid user data" };
                }

                var user = new Users
                {
                    first_name = stringNormalizer.NormalizeName(dto.FirstName),
                    last_name = stringNormalizer.NormalizeName(dto.LastName),
                    email = dto.Email.ToLower(),
                    gender = dto.Gender,
                    mitrial_status = dto.MitrialStatus,
                    mobile_phone = dto.MobilePhone,
                    role_id = role.role_id,
                    password_hash = passwordHasher.HashPassword(dto.Password),
                    is_active = true,
                    email_verified = "Yes",
                    create_date = DateTime.Now,
                    profile_image = dto.ProfileImage
                };

                await database.Users.AddAsync(user);
                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync(
                    "Create User",
                    "Success",
                    $"User {user.first_name} {user.last_name} created"
                );

                return new CreatedResponseDto
                {
                    UserId = user.user_id,
                    Message = "User Created Successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateUserAsync error");
                await logRepository.CreateLogAsync("Create User", "Error", ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<UserDetailsDto>> GetAllActiveUsersAsync()
        {
            try
            {
                var users = await database.Users
                    .Include(r => r.Role)
                    .Where(u => u.is_active)
                    .ToListAsync();

                await logRepository.CreateLogAsync("Get Active Users", "Success", "Retrieved active users list");

                return users.Select(MapToUserDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllActiveUsersAsync error");
                await logRepository.CreateLogAsync("Get Active Users", "Error", ex.Message);
                return Enumerable.Empty<UserDetailsDto>();
            }
        }
        public async Task<IEnumerable<UserDetailsDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await database.Users
                    .Include(r => r.Role)
                    .ToListAsync();

                await logRepository.CreateLogAsync("Get All Users", "Success", "Retrieved all users");

                return users.Select(MapToUserDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllUsersAsync error");
                await logRepository.CreateLogAsync("Get All Users", "Error", ex.Message);
                return Enumerable.Empty<UserDetailsDto>();
            }
        }
        public async Task<UserDetailsDto> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await database.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.user_id == id);

                if (user == null)
                {
                    await logRepository.CreateLogAsync("Get User By Id", "Error", $"User {id} not found");
                    return null;
                }

                await logRepository.CreateLogAsync("Get User By Id", "Success", $"User {id} retrieved");

                return MapToUserDetailsDto(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserByIdAsync error");
                await logRepository.CreateLogAsync("Get User By Id", "Error", ex.Message);
                return null;
            }
        }
        public async Task<bool> InActiveUserAsync(int id)
        {
            try
            {
                var user = await database.Users.FirstOrDefaultAsync(r => r.user_id == id);

                if (user == null)
                {
                    await logRepository.CreateLogAsync("Deactivate User", "Error", $"User {id} not found");
                    return false;
                }

                user.is_active = false;
                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync("Deactivate User", "Success", $"User {id} deactivated");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "InActiveUserAsync error");
                await logRepository.CreateLogAsync("Deactivate User", "Error", ex.Message);
                return false;
            }
        }

        public async Task<bool> IsEmailExistAsync(string email, int id)
        {
            try
            {
                bool exists = await database.Users
                    .AnyAsync(u => u.email.ToLower() == email.ToLower() && u.user_id != id);

                return exists;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IsEmailExistAsync error");
                await logRepository.CreateLogAsync("Check Email Exists", "Error", ex.Message);
                return true;
            }
        }
        public async Task<bool> IsPhoneExistAsync(string phone, int id)
        {
            try
            {
                bool exists = await database.Users
                    .AnyAsync(u => u.mobile_phone == phone && u.user_id != id);

                return exists;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IsPhoneExistAsync error");
                await logRepository.CreateLogAsync("Check Phone Exists", "Error", ex.Message);
                return true;
            }
        }
        public async Task<UserDetailsDto> UpdateUserAsync(UpdateUserDto dto, int id)
        {
            try
            {
                var user = await database.Users.FirstOrDefaultAsync(u => u.user_id == id);

                if (user == null)
                {
                    await logRepository.CreateLogAsync("Update User", "Error", $"User {id} not found");
                    return null;
                }

                if (!string.IsNullOrEmpty(dto.FirstName)) user.first_name = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName)) user.last_name = dto.LastName;
                if (!string.IsNullOrEmpty(dto.MobilePhone)) user.mobile_phone = dto.MobilePhone;
                if (dto.ProfileImage != null) user.profile_image = dto.ProfileImage;

                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync("Update User", "Success", $"User {id} updated");

                return MapToUserDetailsDto(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateUserAsync error");
                await logRepository.CreateLogAsync("Update User", "Error", ex.Message);
                return null;
            }
        }


        private UserDetailsDto MapToUserDetailsDto(Users d)
        {
            return new UserDetailsDto
            {
                Id = d.user_id,
                FirstName = d.first_name,
                LastName = d.last_name,
                Email = d.email,
                MobilePhone = d.mobile_phone,
                Gender = d.gender,
                ProfileImage = d.profile_image,
                DateOfBirth = d.date_of_birth,
                CreateDate = d.create_date,
                IsActive = d.is_active
            };
        }
    }
}
