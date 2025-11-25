using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<DoctorRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly IPasswordHasherRepository passwordHasher;
        private readonly ILogRepository logRepository;

        public DoctorRepository(
            Medibook_Context database,
            ILogger<DoctorRepository> logger,
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

        public async Task<CreatedResponseDto> CreateDoctorAsync(CreateDoctorDto dto)
        {
            try
            {
                var role = await database.Roles
                    .FirstOrDefaultAsync(r => r.role_name.ToLower() == "doctor");

                if (role == null)
                {
                    string msg = "Doctor role not found in database.";
                    logger.LogError(msg);
                    await logRepository.CreateLogAsync("Create Doctor", "Error", msg);
                    return new CreatedResponseDto { Message = msg };
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

                await database.Users.AddAsync(user);
                await database.SaveChangesAsync();

                var doctor = new Doctors
                {
                    user_id = user.user_id,
                    bio = dto.Bio,
                    specialization = dto.Specialization,
                    Type = dto.Type,
                    experience_years = dto.ExperienceYears
                };

                await database.Doctors.AddAsync(doctor);
                await database.SaveChangesAsync();

                string successMsg = $"Doctor {user.first_name} {user.last_name} created successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Create Doctor", "Success", successMsg);

                return new CreatedResponseDto
                {
                    UserId = user.user_id,
                    TypeId = doctor.doctor_id,
                    Message = successMsg
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating doctor");
                await logRepository.CreateLogAsync("Create Doctor", "Error", ex.Message);

                return new CreatedResponseDto { Message = ex.Message };
            }
        }
        public async Task<IEnumerable<DoctorDetailsDto>> GetAllDoctorsAsync()
        {
            try
            {
                var doctors = await database.Doctors
                    .Include(d => d.Users)
                    .ToListAsync();

                string msg = $"Fetched {doctors.Count} doctors.";
                logger.LogInformation(msg);
                await logRepository.CreateLogAsync("Get All Doctors", "Success", msg);

                return doctors.Select(MapToDoctorDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all doctors");
                await logRepository.CreateLogAsync("Get All Doctors", "Error", ex.Message);
                return Enumerable.Empty<DoctorDetailsDto>();
            }
        }
        public async Task<IEnumerable<DoctorDetailsDto>> GetAllActiveDoctorsAsync()
        {
            try
            {
                var doctors = await database.Doctors
                    .Include(d => d.Users)
                    .Where(d => d.Users.is_active)
                    .ToListAsync();

                string msg = $"Fetched {doctors.Count} active doctors.";
                logger.LogInformation(msg);
                await logRepository.CreateLogAsync("Get Active Doctors", "Success", msg);

                return doctors.Select(MapToDoctorDetailsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving active doctors");
                await logRepository.CreateLogAsync("Get Active Doctors", "Error", ex.Message);
                return Enumerable.Empty<DoctorDetailsDto>();
            }
        }
        public async Task<DoctorDetailsDto> GetDoctorByIdAsync(int id)
        {
            try
            {
                var doctor = await database.Doctors
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.doctor_id == id);

                if (doctor == null)
                {
                    string msg = $"Doctor with ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Get Doctor By ID", "Error", msg);
                    return null;
                }

                string successMsg = $"Doctor with ID {id} retrieved.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Get Doctor By ID", "Success", successMsg);

                return MapToDoctorDetailsDto(doctor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving doctor with ID {id}");
                await logRepository.CreateLogAsync("Get Doctor By ID", "Error", ex.Message);
                return null;
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
                    string msg = $"Doctor with ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Update Doctor", "Error", msg);
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

                string successMsg = $"Doctor with ID {id} updated successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Update Doctor", "Success", successMsg);

                return MapToDoctorDetailsDto(doctor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating doctor with ID {id}");
                await logRepository.CreateLogAsync("Update Doctor", "Error", ex.Message);
                return null;
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
    }
}
