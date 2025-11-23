using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NursesController : Controller
    {
        private readonly ILogger<NursesController> logger;
        private readonly INurseRepository nurseRepository;

        public NursesController(ILogger<NursesController> logger, INurseRepository nurseRepository)
        {
            this.logger = logger;
            this.nurseRepository = nurseRepository;
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
        public async Task<IActionResult> GetAllActiveNurses()
        {
            try
            {
                var nurses = await nurseRepository.GetAllActiveNursesAsync();
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

        // POST: /api/Nurses/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreatedResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateNurse([FromBody] CreateNurseDto dto)
        {
            try
            {
                var createdNurse = await nurseRepository.CreateNurseAsync(dto);

                if (createdNurse.UserId <= 0)
                    return BadRequest(createdNurse.Message);

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
