using medibook_API.Extensions.DTOs;
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
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUserRepository userRepository;
        private readonly IUserContextService userContextService;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository, IUserContextService userContextService)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.userContextService = userContextService;
        }

        // GET: /api/Users/all
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<UserDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await userRepository.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all users.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Users/active
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<UserDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            try
            {
                var users = await userRepository.GetAllActiveUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving active users.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Users/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID.");

                var user = await userRepository.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving user with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: /api/Users/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreatedResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {

                var existingEmailUser = await userRepository.IsEmailExistAsync(dto.Email, -1);
                var existingPhoneUser = await userRepository.IsPhoneExistAsync(dto.MobilePhone, -1);
                if (existingEmailUser)
                    return BadRequest("Email already exists.");
                if (existingPhoneUser)
                    return BadRequest("Mobile Phone is already exists.");

                var createdUser = await userRepository.CreateUserAsync(dto);

                if (createdUser.UserId <= 0)
                    return BadRequest(createdUser.Message);

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating user.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: /api/Users/update/{id}
        [HttpPut("update/{id:int}")]
        [ProducesResponseType(typeof(UserDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid user ID.");

                if (dto.MobilePhone != null)
                {
                    var existingPhoneUser = await userRepository.IsPhoneExistAsync(dto.MobilePhone, id);
                    if (existingPhoneUser)
                        return BadRequest("Mobile Phone Number already exists.");
                }

                var updatedUser = await userRepository.UpdateUserAsync(dto, id);
                if (updatedUser == null)
                    return NotFound($"User with ID {id} not found.");

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while updating user with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PATCH: /api/Users/{id}/activate
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var result = await userRepository.ActiveUserAsync(id);
            if (!result) return NotFound($"User with ID {id} not found.");
            return Ok(new { Message = "User activated successfully" });
        }

        // PATCH: /api/Users/{id}/deactivate
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var result = await userRepository.InActiveUserAsync(id);
            if (!result) return NotFound($"User with ID {id} not found.");
            return Ok(new { Message = "User deactivated successfully" });
        }

        // GET: /api/Users/current
        [HttpGet("current")]
        [ProducesResponseType(typeof(UserDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var currentUserId = userContextService.GetCurrentUserId();
                
                if (currentUserId <= 0)
                {
                    return Unauthorized("Unable to retrieve current user information from token.");
                }

                var user = await userRepository.GetUserByIdAsync(currentUserId);
                if (user == null)
                {
                    return NotFound($"User with ID {currentUserId} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving current user.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
