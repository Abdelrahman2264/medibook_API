using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace medibook_API.Extensions.Repositories
{
    public class NurseRepository : INurseRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<NurseRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly IPasswordHasherRepository passwordHasher;
        private readonly ILogRepository logRepository;

        public NurseRepository(
            Medibook_Context database,
            ILogger<NurseRepository> logger,
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

        public async Task<CreatedResponseDto> CreateNurseAsync(CreateNurseDto dto)
        {
            try
            {
                var role = await database.Roles
                    .FirstOrDefaultAsync(r => r.role_name.ToLower() == "nurse");

                if (role == null)
                {
                    string msg = "Nurse role not found in database.";
                    logger.LogError(msg);
                    await logRepository.CreateLogAsync("Create Nurse", "Error", msg);

                    return new CreatedResponseDto { Message = msg };
                }
                byte[]? profileImageBytes = null;
                if (!string.IsNullOrEmpty(dto.ProfileImage))
                {
                    try
                    {
                        // Remove data URL prefix if present (e.g., "data:image/png;base64,")
                        string base64String = dto.ProfileImage;
                        if (base64String.Contains(","))
                        {
                            base64String = base64String.Split(',')[1];
                        }
                        profileImageBytes = Convert.FromBase64String(base64String);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to convert base64 profile image to byte array");
                        // Continue without profile image if conversion fails
                    }
                }

                    var user = new Users
                {
                    first_name = stringNormalizer.NormalizeName(dto.FirstName),
                    last_name = stringNormalizer.NormalizeName(dto.LastName),
                    email = dto.Email.ToLower(),
                    mobile_phone = dto.MobilePhone,
                    gender = dto.Gender,
                    mitrial_status = dto.MitrialStatus,
                    profile_image = profileImageBytes,
                    date_of_birth = dto.DateOfBirth,
                    password_hash = passwordHasher.HashPassword(dto.Password),
                    create_date = DateTime.Now,
                    is_active = true,
                    email_verified = "No",
                    role_id = role.role_id
                };

                await database.Users.AddAsync(user);
                await database.SaveChangesAsync();

                var nurse = new Nurses
                {
                    user_id = user.user_id,
                    bio = dto.Bio
                };

                await database.Nurses.AddAsync(nurse);
                await database.SaveChangesAsync();

                string successMsg = $"Nurse {user.first_name} {user.last_name} created successfully.";
                await logRepository.CreateLogAsync("Create Nurse", "Success", successMsg);
                logger.LogInformation(successMsg);

                return new CreatedResponseDto
                {
                    UserId = user.user_id,
                    TypeId = nurse.nurse_id,
                    Message = successMsg
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating nurse");
                await logRepository.CreateLogAsync("Create Nurse", "Error", ex.Message);

                return new CreatedResponseDto { Message = ex.Message };
            }
        }
        public async Task<IEnumerable<NurseDetailsDto>> GetAllNursesAsync()
        {
            try
            {
                var nurses = await database.Nurses
                    .Include(n => n.Users)
                    .ToListAsync();

                string msg = $"Fetched {nurses.Count} nurses.";
                logger.LogInformation(msg);
                await logRepository.CreateLogAsync("Get All Nurses", "Success", msg);

                return nurses.Select(MapToNurseDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all nurses");
                await logRepository.CreateLogAsync("Get All Nurses", "Error", ex.Message);

                return Enumerable.Empty<NurseDetailsDto>();
            }
        }
        public async Task<IEnumerable<NurseDetailsDto>> GetAllActiveNursesAsync()
        {
            try
            {
                var nurses = await database.Nurses
                    .Include(n => n.Users)
                    .Where(n => n.Users.is_active)
                    .ToListAsync();

                string msg = $"Fetched {nurses.Count} active nurses.";
                logger.LogInformation(msg);
                await logRepository.CreateLogAsync("Get Active Nurses", "Success", msg);

                return nurses.Select(MapToNurseDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving active nurses");
                await logRepository.CreateLogAsync("Get Active Nurses", "Error", ex.Message);

                return Enumerable.Empty<NurseDetailsDto>();
            }
        }
        public async Task<NurseDetailsDto> GetNurseByIdAsync(int id)
        {
            try
            {
                var nurse = await database.Nurses
                    .Include(n => n.Users)
                    .FirstOrDefaultAsync(n => n.nurse_id == id);

                if (nurse == null)
                {
                    string msg = $"Nurse with ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Get Nurse By ID", "Error", msg);
                    return null;
                }

                string successMsg = $"Nurse with ID {id} retrieved.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Get Nurse By ID", "Success", successMsg);

                return MapToNurseDetailsDto(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving nurse with ID {id}");
                await logRepository.CreateLogAsync("Get Nurse By ID", "Error", ex.Message);
                return null;
            }
        }
        public async Task<NurseDetailsDto> GetNurseByUserIdAsync(int id)
        {
            try
            {
                var nurse = await database.Nurses
                    .Include(n => n.Users)
                    .FirstOrDefaultAsync(n => n.user_id == id);

                if (nurse == null)
                {
                    string msg = $"Nurse with User ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Get Nurse By User ID", "Error", msg);
                    return null;
                }

                string successMsg = $"Nurse with User ID {id} retrieved.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Get Nurse By User ID", "Success", successMsg);

                return MapToNurseDetailsDto(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving nurse with User ID {id}");
                await logRepository.CreateLogAsync("Get Nurse By User ID", "Error", ex.Message);
                return null;
            }
        }
        public async Task<NurseDetailsDto> UpdateNurseAsync(int id, UpdateNurseDto dto)
        {
            try
            {
                var nurse = await database.Nurses
                    .Include(n => n.Users)
                    .FirstOrDefaultAsync(n => n.nurse_id == id);

                if (nurse == null)
                {
                    string msg = $"Nurse with ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Update Nurse", "Error", msg);
                    return null;
                }

                if (!string.IsNullOrEmpty(dto.FirstName))
                    nurse.Users.first_name = dto.FirstName;

                if (!string.IsNullOrEmpty(dto.LastName))
                    nurse.Users.last_name = dto.LastName;

                if (!string.IsNullOrEmpty(dto.MobilePhone))
                    nurse.Users.mobile_phone = dto.MobilePhone;
                if (!string.IsNullOrEmpty(dto.MitrialStatus))
                    nurse.Users.mitrial_status = dto.MitrialStatus;

                if (dto.ProfileImage != null)
                    try
                    {
                        // Remove data URL prefix if present (e.g., "data:image/png;base64,")
                        string base64String = dto.ProfileImage;
                        if (base64String.Contains(","))
                        {
                            base64String = base64String.Split(',')[1];
                        }
                        nurse.Users.profile_image = Convert.FromBase64String(base64String);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to convert base64 profile image to byte array");
                        // Continue without updating profile image if conversion fails
                    }
                if (!string.IsNullOrEmpty(dto.Bio))
                    nurse.bio = dto.Bio;

                await database.SaveChangesAsync();

                string successMsg = $"Nurse with ID {id} updated successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Update Nurse", "Success", successMsg);

                return MapToNurseDetailsDto(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating nurse with ID {id}");
                await logRepository.CreateLogAsync("Update Nurse", "Error", ex.Message);
                return null;
            }
        }
        private NurseDetailsDto MapToNurseDetailsDto(Nurses n)
        {
            string? profileImageBase64 = null;
            if (n.Users.profile_image != null && n.Users.profile_image.Length > 0)
            {
                try
                {
                    profileImageBase64 = Convert.ToBase64String(n.Users.profile_image);
                    // Add data URL prefix for easy use in frontend
                    profileImageBase64 = $"data:image/jpeg;base64,{profileImageBase64}";
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to convert profile image to base64");
                }
            }
            return new NurseDetailsDto
            {
                UserId = n.Users.user_id,
                FirstName = n.Users.first_name,
                LastName = n.Users.last_name,
                Email = n.Users.email,
                MobilePhone = n.Users.mobile_phone,
                Gender = n.Users.gender,
                ProfileImage = profileImageBase64,
                NurseId = n.nurse_id,
                DateOfBirth = n.Users.date_of_birth,
                CreateDate = n.Users.create_date,
                IsActive = n.Users.is_active,
                Bio = n.bio
            };
        }
    }
}
