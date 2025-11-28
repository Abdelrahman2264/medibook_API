using Microsoft.AspNetCore.Mvc;

namespace medibook_API.Controllers
{
    using global::medibook_API.Extensions.DTOs;
    using global::medibook_API.Extensions.IRepositories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;

    namespace medibook_API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize] // Require JWT token for all endpoints
        public class AppointmentsController : Controller
        {
            private readonly ILogger<AppointmentsController> logger;
            private readonly IAppointmentRepository appointmentRepository;

            public AppointmentsController(ILogger<AppointmentsController> logger, IAppointmentRepository appointmentRepository)
            {
                this.logger = logger;
                this.appointmentRepository = appointmentRepository;
            }

            // GET: /api/Appointments
            [HttpGet("all")]
            [ProducesResponseType(typeof(IEnumerable<AppointmentDetailsDto>), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> GetAllAppointments()
            {
                try
                {
                    var appointments = await appointmentRepository.GetAllAppointmentAsync();
                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error retrieving all appointments.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: /api/Appointments/{id}
            [HttpGet("{id:int}")]
            [ProducesResponseType(typeof(AppointmentDetailsDto), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.NotFound)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> GetAppointmentById(int id)
            {
                try
                {
                    if (id <= 0) return BadRequest("Invalid appointment ID.");

                    var appointment = await appointmentRepository.GetAppointmentByAppoitnmentIdAsync(id);
                    if (appointment == null)
                        return NotFound($"Appointment with ID {id} not found.");

                    return Ok(appointment);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error retrieving appointment with ID {id}.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // POST: /api/Appointments/create
            [HttpPost("create")]
            [ProducesResponseType(typeof(AppointmentResponseDto), (int)HttpStatusCode.Created)]
            [ProducesResponseType((int)HttpStatusCode.BadRequest)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> CreateAppointment([FromBody] CreateAppoinmentDto dto)
            {
                try
                {
                    var response = await appointmentRepository.CreateAppintmentAsync(dto);
                    return CreatedAtAction(nameof(GetAppointmentById), new { id = response.appointment_id }, response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error creating appointment.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // PUT: /api/Appointments/assign
            [HttpPut("assign")]
            [ProducesResponseType((int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.BadRequest)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> AssignAppointment([FromBody] AssignAppoinmentDto dto)
            {
                try
                {
                    var success = await appointmentRepository.AssignAppointmentAsync(dto);
                    if (!success)
                        return BadRequest("Failed to assign appointment. Check appointment, nurse, or room IDs.");

                    return Ok("Appointment assigned successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error assigning appointment.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // PUT: /api/Appointments/cancel
            [HttpPut("cancel")]
            [ProducesResponseType(typeof(AppointmentResponseDto), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.BadRequest)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentDto dto)
            {
                try
                {
                    
                    var response = await appointmentRepository.CancelAppointmentAsync(dto);
                    if (response.appointment_id == 0)
                        return BadRequest(response.message);

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error cancelling appointment.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // PUT: /api/Appointments/close
            [HttpPut("close")]
            [ProducesResponseType((int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.BadRequest)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> CloseAppointment([FromBody] CloseAppointmentDto dto)
            {
                try
                {
                    var success = await appointmentRepository.CloseAppointmentAsync(dto);
                    if (!success)
                        return BadRequest("Failed to close appointment. Check appointment ID.");

                    return Ok("Appointment closed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error closing appointment.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: /api/Appointments/patient/{id}
            [HttpGet("patient/{id:int}")]
            [ProducesResponseType(typeof(IEnumerable<AppointmentDetailsDto>), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> GetAppointmentsByPatientId(int id)
            {
                try
                {
                    var appointments = await appointmentRepository.GetAllAppointmentByPatientIdAsync(id);
                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error retrieving appointments for patient {id}.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: /api/Appointments/doctor/{id}
            [HttpGet("doctor/{id:int}")]
            [ProducesResponseType(typeof(IEnumerable<AppointmentDetailsDto>), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> GetAppointmentsByDoctorId(int id)
            {
                try
                {
                    var appointments = await appointmentRepository.GetAllAppointmentByDoctorIdAsync(id);
                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error retrieving appointments for doctor {id}.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: /api/Appointments/nurse/{id}
            [HttpGet("nurse/{id:int}")]
            [ProducesResponseType(typeof(IEnumerable<AppointmentDetailsDto>), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> GetAppointmentsByNurseId(int id)
            {
                try
                {
                    var appointments = await appointmentRepository.GetAllAppointmentByNurseIdAsync(id);
                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error retrieving appointments for nurse {id}.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: /api/Appointments/available-dates
            [HttpGet("available-dates/{id:int}")]
            [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> GetAllActiveAppointmentDates(int id)
            {
                try
                {
                    var dates = await appointmentRepository.GetAllActiveAppointmentDatesAsync(id);

                    // Format the dates to "yyyy-MM-dd hh:mm tt"
                    var formattedDates = dates.Select(d => d.ToString("yyyy-MM-dd hh:mm tt"));

                    return Ok(formattedDates);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while retrieving available appointment dates.");
                    return StatusCode(500, "Internal server error");
                }
            }

            // GET: /api/Appointments/check-date?time=2025-11-25T10:00:00
            [HttpGet("check-date")]
            [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
            public async Task<IActionResult> CheckAppointmentDate([FromQuery] DateTime time)
            {
                try
                {
                    var isBooked = await appointmentRepository.IfAppointmentDateNotAvailableAsync(time);
                    return Ok(isBooked); // true = not available, false = available
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred while checking appointment availability for {time}.");
                    return StatusCode(500, "Internal server error");
                }
            }
        }
    }

}
