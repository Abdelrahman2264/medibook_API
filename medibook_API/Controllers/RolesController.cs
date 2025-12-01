using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Repositories;
using medibook_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // API prefix
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public class RolesController : Controller
    {
        private readonly ILogger<RolesController> logger;
        private readonly IRolesRepository rolesRepository;
        public RolesController(ILogger<RolesController> logger, IRolesRepository rolesRepository)
        {
            this.logger = logger;
            this.rolesRepository = rolesRepository;
        }

        // GET: /api/Roles
        [HttpGet("all")] // route: /api/Roles/all
        [ProducesResponseType(typeof(IEnumerable<Roles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await rolesRepository.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllRooms: An error occurred while retrieving rooms.");
                return StatusCode(500, "Internal server error");
            }
        }
        // GET: /api/Roles
        [HttpGet("{id:int}")] // route: /api/Roles/{id}
        [ProducesResponseType(typeof(Roles), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid role ID.");
                }
                var role = await rolesRepository.GetRoleByIdAsync(id);

                if (role.role_id <= 0)
                {
                    return NotFound($"Role with ID {id} not found.");
                }
                return Ok(role);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetRoomById: An error occurred while retrieving room with Id {id}.");
                return StatusCode(500, "Internal server error");
            }
        }


        // POST: /api/Roles/Create
        [HttpPost("create")]  // route: /api/Roles/Create
        [ProducesResponseType(typeof(Roles), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateRole([FromBody] Roles role)
        {
            try
            {
                var role_exist = await rolesRepository.IsRoleExistAsync(role.role_name);
                if (role_exist == true)
                {
                    return BadRequest($"Role With Name  {role.role_name}  is already exist");
                }
                var createdRole = await rolesRepository.CreateNewRoleAsync(role);
                if (createdRole.role_id <= 0)
                {
                    return BadRequest("Failed to create room.");
                }

                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.role_id }, createdRole);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateRole: An error occurred while creating the role.");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
