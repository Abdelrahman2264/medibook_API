using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController : Controller
    {
        private readonly ILogger<DoctorsController> logger;
        private readonly IDoctorRepository doctorRepository;

        public DoctorsController(ILogger<DoctorsController> logger, IDoctorRepository doctorRepository)
        {
            this.logger = logger;
            this.doctorRepository = doctorRepository;
        }

        // GET: /api/Doctor/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<DoctorDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await doctorRepository.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllDoctors: An error occurred while retrieving doctors.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Doctor/active
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<DoctorDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllActiveDoctors()
        {
            try
            {
                var doctors = await doctorRepository.GetAllActiveDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllActiveDoctors: An error occurred while retrieving active doctors.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Doctor/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DoctorDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid doctor ID.");

                var doctor = await doctorRepository.GetDoctorByIdAsync(id);

                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found.");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetDoctorById: An error occurred while retrieving doctor with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Doctor/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreatedResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto dto)
        {
            try
            {
                var result = await doctorRepository.CreateDoctorAsync(dto);

                if (result.TypeId <= 0 || result.UserId <= 0)
                    return BadRequest(result.Message);

                return CreatedAtAction(nameof(GetDoctorById), new { id = result.TypeId }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateDoctor: An error occurred while creating a doctor.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: /api/Doctor/update/{id}
        [HttpPut("update/{id:int}")]
        [ProducesResponseType(typeof(DoctorDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid doctor ID.");

                var updatedDoctor = await doctorRepository.UpdateDoctorAsync(id, dto);

                if (updatedDoctor == null)
                    return NotFound($"Doctor with ID {id} not found or update failed.");

                return Ok(updatedDoctor);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"UpdateDoctor: An error occurred while updating doctor with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
