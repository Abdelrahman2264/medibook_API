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

        public UserRepository(Medibook_Context database,
            ILogger<UserRepository> logger,
            StringNormalizer stringNormalizer,
            IPasswordHasherRepository passwordHasher)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
            this.passwordHasher = passwordHasher;
        }
        public async Task<bool> ActiveUserAsync(int id)
        {
            try
            {
                var user = await database.Users.FirstOrDefaultAsync(r => r.user_id == id);
                if (user == null)
                {
                    logger.LogWarning("ActiveUserAsync: User with ID {UserId} not found.", id);
                    return false;
                }
                user.is_active = true;
                database.Users.Update(user);
                await database.SaveChangesAsync();
                logger.LogInformation("ActiveUserAsync: User with ID {UserId} deactivated successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActiveUserAsync: An error occurred while deactivating the user.");
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
                    logger.LogWarning("User Role Is Not Found");
                    CreatedResponseDto response = new CreatedResponseDto
                    {
                        Message = "User Role Is Not Found"
                    };
                    return response;

                }
                if (dto == null)
                {
                    logger.LogWarning("User Data Is Invaild");
                    CreatedResponseDto response = new CreatedResponseDto
                    {
                        Message = "Invaild Data User Is Required"
                    };
                    return response;
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

                var userEntry = await database.Users.AddAsync(user);
                await database.SaveChangesAsync();
                return new CreatedResponseDto
                {
                    UserId = user.user_id,
                    Message = "User Created Successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a new user.");
                throw;
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
                IsActive = d.is_active,
            };
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllActiveUsersAsync()
        {
            try
            {
                logger.LogInformation("Fetching all active users with user data...");

                var users = await database.Users
                    .Include(d => d.Role)
                    .Where(u => u.is_active)
                    .ToListAsync();

                if (!users.Any())
                {
                    logger.LogWarning("No active users found in the system.");
                }
                else
                {
                    logger.LogInformation("Successfully retrieved {Count}active users.", users.Count);
                }

                return users.Select(MapToUserDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all active users.");
                return Enumerable.Empty<UserDetailsDto>();
            }
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllUsersAsync()
        {
            try
            {
                logger.LogInformation("Fetching all users with user data...");

                var users = await database.Users
                    .Include(d => d.Role)
                    .ToListAsync();

                if (!users.Any())
                {
                    logger.LogWarning("No users found in the system.");
                }
                else
                {
                    logger.LogInformation("Successfully retrieved {Count} users.", users.Count);
                }

                return users.Select(MapToUserDetailsDto);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all users.");
                return Enumerable.Empty<UserDetailsDto>();
            }
        }

        public async Task<UserDetailsDto> GetUserByIdAsync(int id)
        {
            try
            {
                logger.LogInformation("Fetching user data...");

                var user = await database.Users
                    .Include(d => d.Role)
                    .FirstOrDefaultAsync(u => u.user_id == id);

                if (user == null)
                {
                    logger.LogWarning("No user found in the system.");
                    return null;
                }
                else
                {
                    logger.LogInformation("Successfully retrieved  user.");
                }

                return MapToUserDetailsDto(user);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving  user.");
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
                    logger.LogWarning("InactiveUSerAsync: User with ID {UserId} not found.", id);
                    return false;
                }
                user.is_active = false;
                database.Users.Update(user);
                await database.SaveChangesAsync();
                logger.LogInformation("InactiveUserAsync: User with ID {UserId} deactivated successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "InactiveUserAsync: An error occurred while deactivating the user.");
                return false;
            }
        }

        public async Task<bool> IsEmailExistAsync(string email, int id)
        {
            try
            {
                var user = await database.Users.FirstOrDefaultAsync(u => u.email.ToLower() == email.ToLower() && u.user_id != id);
                if (user == null)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in check if email exist {ex.Message}");
                return true;
            }
        }

        public async Task<bool> IsPhoneExistAsync(string phone, int id)
        {
            try
            {
                var user = await database.Users.FirstOrDefaultAsync(u => u.mobile_phone == phone && u.user_id != id);
                if (user == null)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in check if phone number exist {ex.Message}");
                return true;
            }
        }

        public async Task<UserDetailsDto> UpdateUserAsync(UpdateUserDto dto, int id)
        {
            try
            {
                var user = await database.Users
                    .Include(d => d.Role)
                    .FirstOrDefaultAsync(d => d.user_id == id);

                if (user == null)
                {
                    logger.LogWarning($"user with ID {id} not found.");
                    return null;
                }

                if (!string.IsNullOrEmpty(dto.FirstName))
                    user.first_name = dto.FirstName;

                if (!string.IsNullOrEmpty(dto.LastName))
                    user.last_name = dto.LastName;

                if (!string.IsNullOrEmpty(dto.MobilePhone))
                    user.mobile_phone = dto.MobilePhone;

                if (dto.ProfileImage != null)
                    user.profile_image = dto.ProfileImage;

                await database.SaveChangesAsync();

                logger.LogInformation($"user with ID {id} updated successfully.");

                return MapToUserDetailsDto(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating user with ID {id}");
                return null;
            }
        }
    }
}
