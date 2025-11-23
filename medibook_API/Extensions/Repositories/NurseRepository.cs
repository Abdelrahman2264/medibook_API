using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class NurseRepository : INurseRepository
    {

        private readonly Medibook_Context database;
        private readonly ILogger<NurseRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly IPasswordHasherRepository passwordHasher;
        public NurseRepository(Medibook_Context database,
            ILogger<NurseRepository> logger,
            StringNormalizer stringNormalizer,
            IPasswordHasherRepository passwordHasher)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
            this.passwordHasher = passwordHasher;
        }


        public async Task<CreatedResponseDto> CreateNurseAsync(CreateNurseDto dto)
        {
            try
            {
                var role = await database.Roles
                    .FirstOrDefaultAsync(r => r.role_name.ToLower() == "nurse");

                if (role == null)
                {
                    logger.LogError("Nurse role not found in the database.");
                    return new CreatedResponseDto
                    {
                        Message = "Nurse role not found"
                    };
                }

                var user = new Users
                {
                    first_name = stringNormalizer.NormalizeName(dto.FirstName),
                    last_name = stringNormalizer.NormalizeName(dto.LastName),
                    email = dto.Email.ToLower(),
                    mobile_phone = dto.MobilePhone,
                    gender = dto.Gender,
                    mitrial_status = dto.mitrial_status,
                    profile_image = dto.ProfileImage,
                    date_of_birth = dto.DateOfBirth,
                    password_hash = passwordHasher.HashPassword(dto.Password),
                    create_date = DateTime.Now,
                    is_active = true,
                    email_verified = "No",
                    role_id = role.role_id
                };

                var userEntry = await database.Users.AddAsync(user);
                await database.SaveChangesAsync();

                int newUserId = userEntry.Entity.user_id;

                var Nurse = new Nurses
                {
                    user_id = newUserId,
                    bio = dto.Bio,

                };

                var nurseEntry = await database.Nurses.AddAsync(Nurse);
                await database.SaveChangesAsync();

                return new CreatedResponseDto
                {
                    UserId = newUserId,
                    TypeId = nurseEntry.Entity.nurse_id,
                    Message = "Nurse created successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new nurse.");
                return new CreatedResponseDto
                {
                    Message = "Error while creating nurse"
                };
            }
        }
        public async Task<IEnumerable<NurseDetailsDto>> GetAllNursesAsync()
        {
            try
            {
                logger.LogInformation("Fetching all nurses with user data...");

                var nurses = await database.Nurses
                    .Include(d => d.Users)
                    .ToListAsync();

                if (!nurses.Any())
                {
                    logger.LogWarning("No nurses found in the system.");
                }
                else
                {
                    logger.LogInformation("Successfully retrieved {Count} nurses.", nurses.Count);
                }

                return nurses.Select(MapToNurseDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all doctors.");
                return Enumerable.Empty<NurseDetailsDto>();
            }
        }
        public async Task<IEnumerable<NurseDetailsDto>> GetAllActiveNursesAsync()
        {
            try
            {
                logger.LogInformation("Fetching all ACTIVE nurses with user data...");

                var nurses = await database.Nurses
                    .Include(d => d.Users)
                    .Where(d => d.Users.is_active)
                    .ToListAsync();

                if (!nurses.Any())
                {
                    logger.LogWarning("No active nurses found.");
                }
                else
                {
                    logger.LogInformation("Successfully retrieved {Count} nurses.", nurses.Count);
                }

                return nurses.Select(MapToNurseDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving active nurses.");
                return Enumerable.Empty<NurseDetailsDto>();
            }
        }
        private NurseDetailsDto MapToNurseDetailsDto(Nurses d)
        {
            return new NurseDetailsDto
            {
                UserId = d.Users.user_id,
                FirstName = d.Users.first_name,
                LastName = d.Users.last_name,
                Email = d.Users.email,
                MobilePhone = d.Users.mobile_phone,
                Gender = d.Users.gender,
                ProfileImage = d.Users.profile_image,
                DateOfBirth = d.Users.date_of_birth,
                CreateDate = d.Users.create_date,
                IsActive = d.Users.is_active,
                Bio = d.bio
            };
        }
        public async Task<NurseDetailsDto> GetNurseByIdAsync(int id)
        {
            try
            {
                logger.LogInformation("Fetching nurse with ID {NurseId} including user data...", id);

                var nurse = await database.Nurses
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.nurse_id == id);

                if (nurse == null)
                {
                    logger.LogWarning("nurse with ID {NurseId} was not found.", id);
                    return null;
                }

                logger.LogInformation("Nurse with ID {NurseId} retrieved successfully.", id);
                return MapToNurseDetailsDto(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving doctor with ID {DoctorId}.", id);
                throw;
            }
        }
        public async Task<NurseDetailsDto> UpdateNurseAsync(int id, UpdateNurseDto dto)
        {
            try
            {
                var nurse = await database.Nurses
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.nurse_id == id);

                if (nurse == null)
                {
                    logger.LogWarning($"Nurse with ID {id} not found.");
                    return null;
                }

                if (!string.IsNullOrEmpty(dto.FirstName))
                    nurse.Users.first_name = dto.FirstName;

                if (!string.IsNullOrEmpty(dto.LastName))
                    nurse.Users.last_name = dto.LastName;

                if (!string.IsNullOrEmpty(dto.MobilePhone))
                    nurse.Users.mobile_phone = dto.MobilePhone;

                if (dto.profile_image != null)
                    nurse.Users.profile_image = dto.profile_image;

                if (!string.IsNullOrEmpty(dto.Bio))
                    nurse.bio = dto.Bio;

                await database.SaveChangesAsync();

                logger.LogInformation($"Nurse with ID {id} updated successfully.");

                return MapToNurseDetailsDto(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating nurse with ID {id}");
                return null;
            }
        }

    }
}
