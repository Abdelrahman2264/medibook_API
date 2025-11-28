using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<AppointmentRepository> logger;
        private readonly ILogRepository logRepository;
        private readonly INotificationService notificationService;
        private readonly IUserContextService userContextService;

        public AppointmentRepository(
            Medibook_Context database,
            ILogger<AppointmentRepository> logger,
            ILogRepository logRepository,
            INotificationService notificationService,
            IUserContextService userContextService)
        {
            this.database = database;
            this.logger = logger;
            this.logRepository = logRepository;
            this.notificationService = notificationService;
            this.userContextService = userContextService;
        }

        public async Task<bool> AssignAppointmentAsync(AssignAppoinmentDto dto)
        {
            try
            {
                var appointment = await database.Appointments
                    .FirstOrDefaultAsync(a => a.appointment_id == dto.appointment_id);
                if (appointment == null)
                {
                    logger.LogWarning($"Appointment with ID {dto.appointment_id} not found.");
                    await logRepository.CreateLogAsync("ASSIGN_APPOINTMENT", "WARNING",
                        $"Appointment with ID {dto.appointment_id} not found for assignment.");
                    return false;
                }
                appointment.nurse_id = dto.nurse_id;
                appointment.room_id = dto.room_id;
                appointment.status = "Assigned";
                database.Appointments.Update(appointment);
                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync("ASSIGN_APPOINTMENT", "SUCCESS",
                    $"Appointment {dto.appointment_id} assigned to Nurse ID: {dto.nurse_id}, Room ID: {dto.room_id}");

                // Send notifications
                var currentUserId = userContextService.GetCurrentUserId();
                var senderId = currentUserId > 0 ? currentUserId : appointment.doctor_id;

                // Get related user IDs
                var patient = await database.Users.FirstOrDefaultAsync(u => u.user_id == appointment.patient_id);
                var doctor = await database.Doctors.FirstOrDefaultAsync(d => d.doctor_id == appointment.doctor_id);
                var nurse = await database.Nurses.FirstOrDefaultAsync(n => n.nurse_id == dto.nurse_id);

                var appointmentDate = appointment.appointment_date.ToString("yyyy-MM-dd hh:mm tt");

                // Notify doctor
                if (doctor != null)
                {
                    var doctorMessage = $"Appointment {dto.appointment_id} has been assigned. Date: {appointmentDate}";
                    await notificationService.SendNotificationToDoctorAsync(senderId, doctor.user_id, doctorMessage);
                }

                // Notify patient
                if (patient != null)
                {
                    var patientMessage = $"Your appointment {dto.appointment_id} has been assigned. Date: {appointmentDate}";
                    await notificationService.SendNotificationToPatientAsync(senderId, patient.user_id, patientMessage);
                }

                // Notify nurse
                if (nurse != null)
                {
                    var nurseMessage = $"You have been assigned to appointment {dto.appointment_id}. Date: {appointmentDate}";
                    await notificationService.SendNotificationToNurseAsync(senderId, nurse.user_id, nurseMessage);
                }

                // Notify all admins
                var adminMessage = $"Appointment {dto.appointment_id} has been assigned. Nurse ID: {dto.nurse_id}, Room ID: {dto.room_id}";
                await notificationService.SendNotificationToAdminsAsync(senderId, adminMessage);

                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(AssignAppointmentAsync)}: {ex.Message}");
                await logRepository.CreateLogAsync("ASSIGN_APPOINTMENT", "ERROR",
                    $"Error assigning appointment {dto.appointment_id}: {ex.Message}");
                throw;

            }
        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(CancelAppointmentDto dto)
        {
            try
            {
                var appointment = await database.Appointments
                    .FirstOrDefaultAsync(a => a.appointment_id == dto.appointment_id);
                if (appointment == null)
                {
                    logger.LogWarning($"Appointment with ID {dto.appointment_id} not found.");
                    await logRepository.CreateLogAsync("CANCEL_APPOINTMENT", "WARNING",
                        $"Appointment with ID {dto.appointment_id} not found for cancellation.");
                    return new AppointmentResponseDto
                    {
                        message = "Appointment not found."
                    };
                }
                appointment.status = "Cancelled";
                database.Appointments.Update(appointment);
                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync("CANCEL_APPOINTMENT", "SUCCESS",
                    $"Appointment {dto.appointment_id} cancelled successfully. Patient ID: {appointment.patient_id}, Doctor ID: {appointment.doctor_id}");

                // Send notifications
                var currentUserId = userContextService.GetCurrentUserId();
                var senderId = currentUserId > 0 ? currentUserId : appointment.patient_id;

                // Get related user IDs
                var patient = await database.Users.FirstOrDefaultAsync(u => u.user_id == appointment.patient_id);
                var doctor = await database.Doctors.FirstOrDefaultAsync(d => d.doctor_id == appointment.doctor_id);
                var appointmentDate = appointment.appointment_date.ToString("yyyy-MM-dd hh:mm tt");

                // Notify doctor
                if (doctor != null)
                {
                    var doctorMessage = $"Appointment {dto.appointment_id} scheduled for {appointmentDate} has been cancelled.";
                    await notificationService.SendNotificationToDoctorAsync(senderId, doctor.user_id, doctorMessage);
                }

                // Notify patient
                if (patient != null && senderId != patient.user_id)
                {
                    var patientMessage = $"Your appointment {dto.appointment_id} scheduled for {appointmentDate} has been cancelled.";
                    await notificationService.SendNotificationToPatientAsync(senderId, patient.user_id, patientMessage);
                }

                // Notify all admins
                var adminMessage = $"Appointment {dto.appointment_id} has been cancelled. Patient ID: {appointment.patient_id}, Doctor ID: {appointment.doctor_id}";
                await notificationService.SendNotificationToAdminsAsync(senderId, adminMessage);

                var response = new AppointmentResponseDto
                {
                    appointment_id = appointment.appointment_id,
                    appointment_date = appointment.appointment_date,
                    appointment_time = appointment.appointment_date.ToString("yyyy-MM-dd hh:mm tt"),
                    message = "Appointment cancelled successfully."
                };
                return response;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(CancelAppointmentAsync)}: {ex.Message}");
                await logRepository.CreateLogAsync("CANCEL_APPOINTMENT", "ERROR",
                    $"Error cancelling appointment {dto.appointment_id}: {ex.Message}");
                throw;
            }


        }

        public async Task<bool> CloseAppointmentAsync(CloseAppointmentDto dto)
        {
            try
            {
                var appointment = await database.Appointments
                    .FirstOrDefaultAsync(a => a.appointment_id == dto.appointment_id);
                if (appointment == null)
                {
                    logger.LogWarning($"Appointment with ID {dto.appointment_id} not found.");
                    await logRepository.CreateLogAsync("CLOSE_APPOINTMENT", "WARNING",
                        $"Appointment with ID {dto.appointment_id} not found for closing.");
                    return false;
                }
                appointment.status = "Completed";
                appointment.notes = dto.notes;
                appointment.medicine = dto.medicine;
                database.Appointments.Update(appointment);
                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync("CLOSE_APPOINTMENT", "SUCCESS",
                    $"Appointment {dto.appointment_id} closed successfully. Patient ID: {appointment.patient_id}, Doctor ID: {appointment.doctor_id}");

                // Send notifications
                var currentUserId = userContextService.GetCurrentUserId();
                var senderId = currentUserId > 0 ? currentUserId : appointment.doctor_id;

                // Get related user IDs
                var patient = await database.Users.FirstOrDefaultAsync(u => u.user_id == appointment.patient_id);
                var doctor = await database.Doctors.FirstOrDefaultAsync(d => d.doctor_id == appointment.doctor_id);
                var nurse = appointment.nurse_id.HasValue
                    ? await database.Nurses.FirstOrDefaultAsync(n => n.nurse_id == appointment.nurse_id.Value)
                    : null;

                var appointmentDate = appointment.appointment_date.ToString("yyyy-MM-dd hh:mm tt");

                // Notify patient
                if (patient != null)
                {
                    var patientMessage = $"Your appointment {dto.appointment_id} scheduled for {appointmentDate} has been completed.";
                    await notificationService.SendNotificationToPatientAsync(senderId, patient.user_id, patientMessage);
                }

                // Notify doctor
                if (doctor != null && senderId != doctor.user_id)
                {
                    var doctorMessage = $"Appointment {dto.appointment_id} has been marked as completed.";
                    await notificationService.SendNotificationToDoctorAsync(senderId, doctor.user_id, doctorMessage);
                }

                // Notify nurse if assigned
                if (nurse != null)
                {
                    var nurseMessage = $"Appointment {dto.appointment_id} has been completed.";
                    await notificationService.SendNotificationToNurseAsync(senderId, nurse.user_id, nurseMessage);
                }

                // Notify all admins
                var adminMessage = $"Appointment {dto.appointment_id} has been completed. Patient ID: {appointment.patient_id}, Doctor ID: {appointment.doctor_id}";
                await notificationService.SendNotificationToAdminsAsync(senderId, adminMessage);

                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(CloseAppointmentAsync)}: {ex.Message}");
                await logRepository.CreateLogAsync("CLOSE_APPOINTMENT", "ERROR",
                    $"Error closing appointment {dto.appointment_id}: {ex.Message}");
                throw;
            }


        }

        public async Task<AppointmentResponseDto> CreateAppintmentAsync(CreateAppoinmentDto dto)
        {
            try
            {
                // Check if appointment date already exists
                var dateExists = await IfAppointmentDateNotAvailableAsync(dto.appointment_date);
                if (dateExists)
                {
                    logger.LogWarning($"Appointment date {dto.appointment_date} is already booked.");
                    await logRepository.CreateLogAsync("CREATE_APPOINTMENT", "WARNING",
                        $"Failed to create appointment: Date {dto.appointment_date} is already booked. Patient ID: {dto.patient_id}, Doctor ID: {dto.doctor_id}");

                    return new AppointmentResponseDto
                    {
                        appointment_id = 0,
                        appointment_date = dto.appointment_date,
                        appointment_time = dto.appointment_date.ToString("yyyy-MM-dd hh:mm tt"),
                        message = "This appointment time slot is already booked. Please choose another time."
                    };
                }

                // Verify patient exists
                var patientExists = await database.Users.AnyAsync(u => u.user_id == dto.patient_id);
                if (!patientExists)
                {
                    logger.LogWarning($"Patient with ID {dto.patient_id} does not exist.");
                    await logRepository.CreateLogAsync("CREATE_APPOINTMENT", "ERROR",
                        $"Failed to create appointment: Patient with ID {dto.patient_id} does not exist.");

                    return new AppointmentResponseDto
                    {
                        appointment_id = 0,
                        appointment_date = dto.appointment_date,
                        appointment_time = dto.appointment_date.ToString("yyyy-MM-dd hh:mm tt"),
                        message = "Patient not found."
                    };
                }

                // Verify doctor exists
                var doctorExists = await database.Doctors.AnyAsync(d => d.doctor_id == dto.doctor_id);
                if (!doctorExists)
                {
                    logger.LogWarning($"Doctor with ID {dto.doctor_id} does not exist.");
                    await logRepository.CreateLogAsync("CREATE_APPOINTMENT", "ERROR",
                        $"Failed to create appointment: Doctor with ID {dto.doctor_id} does not exist.");

                    return new AppointmentResponseDto
                    {
                        appointment_id = 0,
                        appointment_date = dto.appointment_date,
                        appointment_time = dto.appointment_date.ToString("yyyy-MM-dd hh:mm tt"),
                        message = "Doctor not found."
                    };
                }

                var appointment = new Appointments
                {
                    patient_id = dto.patient_id,
                    doctor_id = dto.doctor_id,
                    appointment_date = dto.appointment_date,
                    status = "Scheduled",
                    create_date = DateTime.UtcNow
                };
                database.Appointments.Add(appointment);
                await database.SaveChangesAsync();

                DateTime date = appointment.appointment_date;
                int remainder = date.Minute % 15;
                DateTime roundedDate = remainder < 8
                    ? date.AddMinutes(-remainder)
                    : date.AddMinutes(15 - remainder);

                // Log successful appointment creation
                await logRepository.CreateLogAsync("CREATE_APPOINTMENT", "SUCCESS",
                    $"Appointment created successfully. Appointment ID: {appointment.appointment_id}, Patient ID: {dto.patient_id}, Doctor ID: {dto.doctor_id}, Date: {roundedDate}");

                // Send notifications
                var currentUserId = userContextService.GetCurrentUserId();
                var senderId = currentUserId > 0 ? currentUserId : dto.patient_id;

                // Get doctor user ID
                var doctor = await database.Doctors
                    .FirstOrDefaultAsync(d => d.doctor_id == dto.doctor_id);

                if (doctor != null)
                {
                    var doctorMessage = $"New appointment scheduled for {roundedDate:yyyy-MM-dd hh:mm tt}. Patient: {senderId}";
                    await notificationService.SendNotificationToDoctorAsync(senderId, doctor.user_id, doctorMessage);
                }

                // Send to all admins
                var adminMessage = $"New appointment created. Appointment ID: {appointment.appointment_id}, Date: {roundedDate:yyyy-MM-dd hh:mm tt}";
                await notificationService.SendNotificationToAdminsAsync(senderId, adminMessage);

                var response = new AppointmentResponseDto
                {
                    appointment_id = appointment.appointment_id,
                    appointment_date = roundedDate,
                    appointment_time = roundedDate.ToString("yyyy-MM-dd hh:mm tt"),
                    message = $"Appointment created successfully. You can be there at {roundedDate:hh:mm tt}."
                };

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(CreateAppintmentAsync)}: {ex.Message}");
                await logRepository.CreateLogAsync("CREATE_APPOINTMENT", "ERROR",
                    $"Error creating appointment: {ex.Message}. Patient ID: {dto.patient_id}, Doctor ID: {dto.doctor_id}");
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentAsync()
        {
            try
            {
                // Use proper Include statements with left joins for nullable relationships
                var appointments = await database.Appointments
                    .Include(a => a.Patients)
                    .Include(a => a.Doctors)
                        .ThenInclude(d => d.Users)
                    .Include(a => a.Nurses)
                        .ThenInclude(n => n.Users)
                    .Include(a => a.Rooms)
                    .ToListAsync();

                logger.LogInformation($"Found {appointments.Count} appointments in database");

                var appointmentDtos = appointments.Select(a => MapToNurseDetailsDto(a)).ToList();

                logger.LogInformation($"Mapped {appointmentDtos.Count} appointment DTOs");

                return appointmentDtos;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetAllAppointmentAsync)}: {ex.Message}");
                throw;
            }
        }
        public async Task TestDatabaseConnection()
        {
            try
            {
                // Test if database is accessible
                var canConnect = await database.Database.CanConnectAsync();
                Console.WriteLine($"Can connect to database: {canConnect}");

                // Test basic count
                var appointmentCount = await database.Appointments.CountAsync();
                Console.WriteLine($"Appointment count: {appointmentCount}");

                // Test if any data exists in related tables
                var patientsCount = await database.Users.CountAsync();
                var doctorsCount = await database.Doctors.CountAsync();
                var nursesCount = await database.Nurses.CountAsync();
                var roomsCount = await database.Rooms.CountAsync();

                Console.WriteLine($"Patients: {patientsCount}, Doctors: {doctorsCount}, Nurses: {nursesCount}, Rooms: {roomsCount}");

                // List all appointments with basic info
                var allAppointments = await database.Appointments.ToListAsync();
                foreach (var appt in allAppointments)
                {
                    Console.WriteLine($"Appt ID: {appt.appointment_id}, Date: {appt.appointment_date}, Status: {appt.status}, Patient: {appt.patient_id}, Doctor: {appt.doctor_id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentByPatientIdAsync(int id)
        {
            try
            {

                var appointments = await database.Appointments
                    .Include(a => a.Patients)
                    .Include(a => a.Doctors).ThenInclude(d => d.Users)
                    .Include(a => a.Nurses).ThenInclude(n => n.Users)
                    .Include(a => a.Rooms)
                    .Where(a => a.patient_id == id)
                    .ToListAsync();
                var appointmentDtos = appointments.Select(a => MapToNurseDetailsDto(a));
                return appointmentDtos;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAllAppointmentByPatientIdAsync)}: {ex.Message}");
                //await logRepository.CreateLogAsync(nameof(AppointmentRepository), nameof(GetAllAppointmentByUserIdAsync), ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentByNurseIdAsync(int id)
        {
            try
            {

                var appointments = await database.Appointments
                    .Include(a => a.Patients)
                    .Include(a => a.Doctors).ThenInclude(d => d.Users)
                    .Include(a => a.Nurses).ThenInclude(n => n.Users)
                    .Include(a => a.Rooms)
                    .Where(a => a.nurse_id == id)
                    .ToListAsync();
                var appointmentDtos = appointments.Select(a => MapToNurseDetailsDto(a));
                return appointmentDtos;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAllAppointmentByPatientIdAsync)}: {ex.Message}");
                //await logRepository.CreateLogAsync(nameof(AppointmentRepository), nameof(GetAllAppointmentByUserIdAsync), ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentByDoctorIdAsync(int id)
        {
            try
            {

                var appointments = await database.Appointments
                    .Include(a => a.Patients)
                    .Include(a => a.Doctors).ThenInclude(d => d.Users)
                    .Include(a => a.Nurses).ThenInclude(n => n.Users)
                    .Include(a => a.Rooms)
                    .Where(a => a.doctor_id == id)
                    .ToListAsync();
                var appointmentDtos = appointments.Select(a => MapToNurseDetailsDto(a));
                return appointmentDtos;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAllAppointmentByDoctorIdAsync)}: {ex.Message}");
                //await logRepository.CreateLogAsync(nameof(AppointmentRepository), nameof(GetAllAppointmentByUserIdAsync), ex.Message);
                throw;
            }
        }

        public async Task<AppointmentDetailsDto> GetAppointmentByAppoitnmentIdAsync(int id)
        {
            try
            {
                var appointment = await database.Appointments
                    .Include(a => a.Patients)
                    .Include(a => a.Doctors).ThenInclude(d => d.Users)
                    .Include(a => a.Nurses).ThenInclude(n => n.Users)
                    .Include(a => a.Rooms)
                    .FirstOrDefaultAsync(a => a.appointment_id == id);
                if (appointment == null)
                {
                    logger.LogWarning($"Appointment with ID {id} not found.");
                    return null;
                }
                var appointmentDto = MapToNurseDetailsDto(appointment);
                return appointmentDto;

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAppointmentByAppoitnmentIdAsync)}: {ex.Message}");
                //await logRepository.CreateLogAsync(nameof(AppointmentRepository), nameof(GetAppointmentByAppoitnmentIdAsync), ex.Message);
                throw;

            }
        }
        public async Task<IEnumerable<DateTime>> GetAllActiveAppointmentDatesAsync(int doctorid)
        {
            try
            {
                var now = DateTime.Now;

                // Start from current day at 8:00 AM
                var startDate = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);

                // End of the current month at 6:00 PM
                var endDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 18, 0, 0);

                // Get all existing appointments in the remaining days of the current month
                var bookedAppointments = await database.Appointments
                    .Where(a => a.appointment_date >= startDate && a.appointment_date <= endDate && a.doctor_id == doctorid)
                    .Select(a => a.appointment_date)
                    .ToListAsync();

                var availableSlots = new List<DateTime>();

                // Generate slots from startDate to endDate with 45-minute intervals
                for (var date = startDate; date <= endDate; date = date.AddMinutes(45))
                {
                    if (!bookedAppointments.Contains(date))
                    {
                        availableSlots.Add(date);
                    }
                }

                return availableSlots;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetAllActiveAppointmentDatesAsync)}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IfAppointmentDateNotAvailableAsync(DateTime time)
        {
            try
            {
                // Check if any appointment exists at the given time
                var exists = await database.Appointments
                    .AnyAsync(a => a.appointment_date == time);

                // Return true if the slot is already booked, false if available
                return exists;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(IfAppointmentDateNotAvailableAsync)}: {ex.Message}");
                throw;
            }
        }


        private AppointmentDetailsDto MapToNurseDetailsDto(Appointments n)
        {
            return new AppointmentDetailsDto
            {
                AppointmentDate = n.appointment_date,
                AppointmentId = n.appointment_id, // Add this
                PatientId = n.patient_id,
                PatientFirstName = n.Patients?.first_name ?? "N/A",
                PatientLastName = n.Patients?.last_name ?? "N/A",
                PatientGender = n.Patients?.gender ?? "N/A",
                PatientMartialStatus = n.Patients?.mitrial_status ?? "N/A",
                PatientMobilePhone = n.Patients?.mobile_phone ?? "N/A",
                DoctorId = n.doctor_id,
                DoctorFirstName = n.Doctors?.Users?.first_name ?? "N/A",
                DoctorLastName = n.Doctors?.Users?.last_name ?? "N/A",
                DoctorGender = n.Doctors?.Users?.gender ?? "N/A",
                DoctorMobilePhone = n.Doctors?.Users?.mobile_phone ?? "N/A",
                DoctorType = n.Doctors?.Type ?? "N/A",
                DoctorSpecialization = n.Doctors?.specialization ?? "N/A",

                NurseId = n.nurse_id,
                NurseFirstName = n.Nurses?.Users?.first_name ?? "Not assigned",
                NurseLastName = n.Nurses?.Users?.last_name ?? "Not assigned",
                NurseGender = n.Nurses?.Users?.gender ?? "Not assigned",

                RoomId = n.room_id,
                RoomName = n.Rooms?.room_name ?? "Not assigned",
                RoomType = n.Rooms?.room_type ?? "Not assigned",

                Status = n.status,
                Medicine = n.medicine,
                Notes = n.notes,
            };
        }

    }
}
