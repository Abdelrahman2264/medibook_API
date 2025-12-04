using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.Helpers;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NursesController : Controller
    {
        private readonly ILogger<NursesController> logger;
        private readonly INurseRepository nurseRepository;
        private readonly IUserRepository userRepository;
        private readonly ISignalRService signalRService;

        public NursesController(
            ILogger<NursesController> logger, 
            INurseRepository nurseRepository,
            IUserRepository userRepository,
            ISignalRService signalRService)
        {
            this.logger = logger;
            this.nurseRepository = nurseRepository;
            this.userRepository = userRepository;
            this.signalRService = signalRService;
        }

        // GET: /api/Nurses/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<NurseDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllNurses()
        {
            try
            {
                var nurses = await nurseRepository.GetAllNursesAsync();
                return Ok(nurses);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all nurses.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Nurses/active
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<NurseDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllActiveNurses([FromQuery] DateTime? appointmentDate)
        {
            try
            {
                IEnumerable<NurseDetailsDto> nurses;
                
                if (appointmentDate.HasValue)
                {
                    nurses = await nurseRepository.GetAllActiveNursesAsync(appointmentDate.Value);
                }
                else
                {
                    nurses = await nurseRepository.GetAllActiveNursesAsync();
                }
                
                return Ok(nurses);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving active nurses.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Nurses/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(NurseDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNurseById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid nurse ID.");

                var nurse = await nurseRepository.GetNurseByIdAsync(id);
                if (nurse == null)
                    return NotFound($"Nurse with ID {id} not found.");

                return Ok(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving nurse with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
        // GET: /api/Nurses/{id}
        [HttpGet("byUserId/{id:int}")]
        [ProducesResponseType(typeof(NurseDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNurseByUserId(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid nurse user ID.");

                var nurse = await nurseRepository.GetNurseByUserIdAsync(id);
                if (nurse == null)
                    return NotFound($"Nurse with user ID {id} not found.");

                return Ok(nurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving nurse with user ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Nurses/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreatedResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateNurse([FromBody] CreateNurseDto dto)
        {
            try
            {
                // Check for duplicate email
                var existingEmailUser = await userRepository.IsEmailExistAsync(dto.Email, -1);
                if (existingEmailUser)
                {
                    logger.LogWarning("CreateNurse: Email {Email} already exists", dto.Email);
                    return BadRequest("Email already exists.");
                }

                // Check for duplicate phone
                var existingPhoneUser = await userRepository.IsPhoneExistAsync(dto.MobilePhone, -1);
                if (existingPhoneUser)
                {
                    logger.LogWarning("CreateNurse: Mobile phone {Phone} already exists", dto.MobilePhone);
                    return BadRequest("Mobile phone already exists.");
                }

                var createdNurse = await nurseRepository.CreateNurseAsync(dto);

                if (createdNurse.UserId <= 0)
                    return BadRequest(createdNurse.Message);

                // Send real-time update via SignalR
                await SignalRHelper.NotifyCreatedAsync(
                    signalRService,
                    "Nurse",
                    new { 
                        NurseId = createdNurse.TypeId,
                        UserId = createdNurse.UserId,
                        Email = dto.Email,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName
                    }
                );

                return CreatedAtAction(nameof(GetNurseById), new { id = createdNurse.TypeId }, createdNurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating nurse.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: /api/Nurses/update/{id}
        [HttpPut("update/{id:int}")]
        [ProducesResponseType(typeof(NurseDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateNurse(int id, [FromBody] UpdateNurseDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid nurse ID.");

                var updatedNurse = await nurseRepository.UpdateNurseAsync(id, dto);
                if (updatedNurse == null)
                    return NotFound($"Nurse with ID {id} not found.");

                // Send real-time update via SignalR
                await SignalRHelper.NotifyUpdatedAsync(
                    signalRService,
                    "Nurse",
                    new { 
                        NurseId = updatedNurse.NurseId,
                        UserId = updatedNurse.UserId,
                        FirstName = updatedNurse.FirstName,
                        LastName = updatedNurse.LastName
                    },
                    updatedNurse.UserId
                );

                return Ok(updatedNurse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while updating nurse with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
