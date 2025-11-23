using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class DoctorRepositroy : IDoctorRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<DoctorRepositroy> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly IPasswordHasherRepository passwordHasher;
        public DoctorRepositroy(Medibook_Context database,
            ILogger<DoctorRepositroy> logger,
            StringNormalizer stringNormalizer,
            IPasswordHasherRepository passwordHasher)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
            this.passwordHasher = passwordHasher;
        }


        public async Task<CreatedResponseDto> CreateDoctorAsync(CreateDoctorDto dto)
        {
            try
            {
                var role = await database.Roles
                    .FirstOrDefaultAsync(r => r.role_name.ToLower() == "doctor");

                if (role == null)
                {
                    logger.LogError("Doctor role not found in the database.");
                    return new CreatedResponseDto
                    {
                        Message = "Doctor role not found"
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

                var doctor = new Doctors
                {
                    user_id = newUserId,
                    bio = dto.Bio,
                    specialization = dto.Specialization,
                    Type = dto.Type,
                    experience_years = dto.ExperienceYears
                };

                var doctorEntry = await database.Doctors.AddAsync(doctor);
                await database.SaveChangesAsync();

                return new CreatedResponseDto
                {
                    UserId = newUserId,
                    TypeId = doctorEntry.Entity.doctor_id,
                    Message = "Doctor created successfully"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new doctor.");
                return new CreatedResponseDto
                {
                    Message = "Error while creating doctor"
                };
            }
        }
        public async Task<IEnumerable<DoctorDetailsDto>> GetAllDoctorsAsync()
        {
            try
            {
                logger.LogInformation("Fetching all doctors with user data...");

                var doctors = await database.Doctors
                    .Include(d => d.Users)
                    .ToListAsync();

                if (!doctors.Any())
                {
                    logger.LogWarning("No doctors found in the system.");
                }
                else
                {
                    logger.LogInformation("Successfully retrieved {Count} doctors.", doctors.Count);
                }

                return doctors.Select(MapToDoctorDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all doctors.");
                return Enumerable.Empty<DoctorDetailsDto>();
            }
        }
        public async Task<IEnumerable<DoctorDetailsDto>> GetAllActiveDoctorsAsync()
        {
            try
            {
                logger.LogInformation("Fetching all ACTIVE doctors with user data...");

                var doctors = await database.Doctors
                    .Include(d => d.Users)
                    .Where(d => d.Users.is_active)
                    .ToListAsync();

                if (!doctors.Any())
                {
                    logger.LogWarning("No active doctors found.");
                }
                else
                {
                    logger.LogInformation("Successfully retrieved {Count} active doctors.", doctors.Count);
                }

                return doctors.Select(MapToDoctorDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving active doctors.");
                return Enumerable.Empty<DoctorDetailsDto>();
            }
        }
        private DoctorDetailsDto MapToDoctorDetailsDto(Doctors d)
        {
            return new DoctorDetailsDto
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
                DoctorId = d.doctor_id,
                Bio = d.bio,
                Specialization = d.specialization,
                Type = d.Type,
                ExperienceYears = d.experience_years
            };
        }
        public async Task<DoctorDetailsDto> GetDoctorByIdAsync(int id)
        {
            try
            {
                logger.LogInformation("Fetching doctor with ID {DoctorId} including user data...", id);

                var doctor = await database.Doctors
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.doctor_id == id);

                if (doctor == null)
                {
                    logger.LogWarning("Doctor with ID {DoctorId} was not found.", id);
                    return null;
                }

                logger.LogInformation("Doctor with ID {DoctorId} retrieved successfully.", id);
                return MapToDoctorDetailsDto(doctor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving doctor with ID {DoctorId}.", id);
                throw;
            }
        }
        public async Task<DoctorDetailsDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            try
            {
                var doctor = await database.Doctors
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.doctor_id == id);

                if (doctor == null)
                {
                    logger.LogWarning($"Doctor with ID {id} not found.");
                    return null;
                }

                if (!string.IsNullOrEmpty(dto.Bio))
                    doctor.bio = dto.Bio;

                if (!string.IsNullOrEmpty(dto.Specialization))
                    doctor.specialization = dto.Specialization;

                if (!string.IsNullOrEmpty(dto.Type))
                    doctor.Type = dto.Type;

                if (dto.ExperienceYears.HasValue)
                    doctor.experience_years = dto.ExperienceYears.Value;

                if (!string.IsNullOrEmpty(dto.FirstName))
                    doctor.Users.first_name = dto.FirstName;

                if (!string.IsNullOrEmpty(dto.LastName))
                    doctor.Users.last_name = dto.LastName;

                if (!string.IsNullOrEmpty(dto.MobilePhone))
                    doctor.Users.mobile_phone = dto.MobilePhone;

                if (dto.profile_image != null)
                    doctor.Users.profile_image = dto.profile_image;

                await database.SaveChangesAsync();

                logger.LogInformation($"Doctor with ID {id} updated.");
                return MapToDoctorDetailsDto(doctor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating doctor with ID {id}");
                return null;
            }
        }


    }
}
