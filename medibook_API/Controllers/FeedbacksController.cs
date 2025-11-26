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
    public class FeedbacksController : Controller
    {
        private readonly ILogger<FeedbacksController> _logger;
        private readonly IFeedBackRepository _feedbackRepository;

        public FeedbacksController(ILogger<FeedbacksController> logger, IFeedBackRepository feedbackRepository)
        {
            _logger = logger;
            _feedbackRepository = feedbackRepository;
        }

        // POST: /api/Feedbacks/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(FeedbackResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(FeedbackResponseDto), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Invalid request data.",
                        Success = false
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Validation failed. Please check your input.",
                        Success = false
                    });
                }

                var result = await _feedbackRepository.CreateFeedbackAsync(dto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return CreatedAtAction(nameof(GetFeedbackById), new { id = result.FeedbackId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating feedback.");
                return StatusCode(500, new FeedbackResponseDto
                {
                    FeedbackId = 0,
                    Message = "Internal server error occurred.",
                    Success = false
                });
            }
        }

        // PUT: /api/Feedbacks/add-doctor-reply
        [HttpPut("add-doctor-reply")]
        [ProducesResponseType(typeof(FeedbackResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(FeedbackResponseDto), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddDoctorReply([FromBody] DoctorReplyDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.DoctorReply))
                {
                    return BadRequest(new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Doctor reply is required.",
                        Success = false
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new FeedbackResponseDto
                    {
                        FeedbackId = 0,
                        Message = "Validation failed. Please check your input.",
                        Success = false
                    });
                }

                var result = await _feedbackRepository.AddDoctorReplyAsync(dto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding doctor reply.");
                return StatusCode(500, new FeedbackResponseDto
                {
                    FeedbackId = 0,
                    Message = "Internal server error occurred.",
                    Success = false
                });
            }
        }

        // DELETE: /api/Feedbacks/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid feedback ID.");
                }

                var result = await _feedbackRepository.DeleteFeedbackAsync(id);

                if (!result)
                {
                    return NotFound($"Feedback with ID {id} not found.");
                }

                return Ok(new { Message = "Feedback deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting feedback {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PATCH: /api/Feedbacks/{id}/toggle-favourite
        [HttpPatch("{id:int}/toggle-favourite")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ToggleFavourite(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid feedback ID.");
                }

                var result = await _feedbackRepository.ToggleFavouriteAsync(id);

                if (!result)
                {
                    return NotFound($"Feedback with ID {id} not found.");
                }

                return Ok(new { Message = "Favourite status updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while toggling favourite for feedback {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Feedbacks/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<FeedbackDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all feedbacks.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Feedbacks/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FeedbackDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid feedback ID.");
                }

                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(id);

                if (feedback == null)
                {
                    return NotFound($"Feedback with ID {id} not found.");
                }

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving feedback {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Feedbacks/doctor/{doctorId}
        [HttpGet("doctor/{doctorId:int}")]
        [ProducesResponseType(typeof(IEnumerable<FeedbackDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFeedbacksByDoctorId(int doctorId)
        {
            try
            {
                if (doctorId <= 0)
                {
                    return BadRequest("Invalid doctor ID.");
                }

                var feedbacks = await _feedbackRepository.GetFeedbacksByDoctorIdAsync(doctorId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving feedbacks for doctor {doctorId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Feedbacks/patient/{patientId}
        [HttpGet("patient/{patientId:int}")]
        [ProducesResponseType(typeof(IEnumerable<FeedbackDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFeedbacksByPatientId(int patientId)
        {
            try
            {
                if (patientId <= 0)
                {
                    return BadRequest("Invalid patient ID.");
                }

                var feedbacks = await _feedbackRepository.GetFeedbacksByPatientIdAsync(patientId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving feedbacks for patient {patientId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Feedbacks/nurse/{nurseId}
        [HttpGet("nurse/{nurseId:int}")]
        [ProducesResponseType(typeof(IEnumerable<FeedbackDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFeedbacksByNurseId(int nurseId)
        {
            try
            {
                if (nurseId <= 0)
                {
                    return BadRequest("Invalid nurse ID.");
                }

                var feedbacks = await _feedbackRepository.GetFeedbacksByNurseIdAsync(nurseId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving feedbacks for nurse {nurseId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: /api/Feedbacks/update
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateFeedback([FromBody] UpdateFeedbackDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Invalid request data.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Validation failed. Please check your input.");
                }

                var result = await _feedbackRepository.UpdateFeedbackAsync(dto);

                if (!result)
                {
                    return NotFound($"Feedback with ID {dto.FeedbackId} not found.");
                }

                return Ok(new { Message = "Feedback updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating feedback {dto?.FeedbackId}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}



