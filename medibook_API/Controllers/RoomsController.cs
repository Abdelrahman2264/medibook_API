using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace medibook_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // API prefix
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository roomRepository;
        private readonly ILogger<RoomsController> logger;

        public RoomsController(IRoomRepository roomRepository, ILogger<RoomsController> logger)
        {
            this.roomRepository = roomRepository;
            this.logger = logger;
        }

        // GET: /api/Rooms
        [HttpGet("all")] // route: /api/Rooms/all
        [ProducesResponseType(typeof(IEnumerable<RoomDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await roomRepository.GetALlRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllRooms: An error occurred while retrieving rooms.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /api/Rooms/active
        [HttpGet("active")] // route: /api/Rooms/active
        [ProducesResponseType(typeof(IEnumerable<RoomDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllActiveRooms()
        {
            try
            {
                var rooms = await roomRepository.GetALlActiveAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllActiveRooms: An error occurred while retrieving active rooms.");
                return StatusCode(500, "Internal server error");
            }
        }
        // GET: /api/Rooms/{id}
        [HttpGet("{id:int}")] // route: /api/Rooms/{id}
        [ProducesResponseType(typeof(RoomDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRoomById(int id)
        {
            try
            {
                var room = await roomRepository.GetRoomByIdAsync(id);
                if (room.RoomId <= 0)
                {
                    return NotFound($"ROOM WITH {id} NOT FOUND");
                }
                return Ok(room);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRoomById: An error occurred while retrieving specific room.");
                return StatusCode(500, "Internal server error");
            }
        }
        // POST: /api/Rooms/Create
        [HttpPost("create")]  // route: /api/Rooms/Create
        [ProducesResponseType(typeof(RoomDetailsDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto room)
        {
            try
            {
                var room_Exist = await roomRepository.IsRoomExist(room.RoomName, room.RoomType, 0);
                if (room_Exist == true)
                {
                    return BadRequest($"Room With Name  {room.RoomName} and Type {room.RoomType} is already exist");
                }
                var createdRoom = await roomRepository.CreateRoomAsync(room);
                if (createdRoom.RoomId <= 0)
                {
                    return BadRequest("Failed to create room.");
                }

                return CreatedAtAction(nameof(GetRoomById), new { id = createdRoom.RoomId }, createdRoom);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateRoom: An error occurred while creating the room.");
                return StatusCode(500, "Internal server error");
            }
        }
        // PUT: /api/Rooms/active/{id}
        [HttpPut("active/{id:int}")]  // route: /api/Rooms/active/{id}
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ActiveRoom(int id)
        {
            try
            {
                var success = await roomRepository.ActiveRoomAsync(id);
                if (success)
                {
                    return Ok("ROOM ACTIVATED CORRECTLY");
                }
                return BadRequest("ROOM FAILED TO ACTIVATE SOMETHING GET WRONG");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActiveRoom: An error occurred while active the room.");
                return StatusCode(500, "Internal server error");
            }
        }
        // PUT: /api/Rooms/inactive/{id}
        [HttpPut("inactive/{id:int}")]  // route: /api/Rooms/inactive/{id}
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> InactiveRoom(int id)
        {
            try
            {
                var success = await roomRepository.InactiveRoomAsync(id);
                if (success)
                {
                    return Ok("ROOM INACTIVATED CORRECTLY");
                }
                return BadRequest("ROOM FAILED TO INACTIVATE SOMETHING GET WRONG");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActiveRoom: An error occurred while inactive the room.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: /api/Rooms/Update/{id}
        [HttpPut("update/{id}")] // route: /api/Rooms/Update/{id}
        [ProducesResponseType(typeof(RoomDetailsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomDetailsDto room)
        {
            try
            {
                if (id != room.RoomId)
                {
                    return BadRequest("Room ID mismatch you can't change room ID.");
                }

                var existingRoom = await roomRepository.GetRoomByIdAsync(id);
                if (existingRoom == null)
                {
                    return NotFound($"Room with ID {id} does not exist.");
                }

                var roomExist = await roomRepository.IsRoomExist(room.RoomName, room.RoomType, room.RoomId);
                if (roomExist)
                {
                    return BadRequest($"Another room with Name '{room.RoomName}' and Type '{room.RoomType}' already exists.");
                }

                var updatedRoom = await roomRepository.UpdateRoomAsync(room);
                if (updatedRoom == null)
                {
                    return BadRequest("Failed to update room.");
                }

                return Ok(updatedRoom);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateRoom: An error occurred while updating the room.");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: /api/Rooms/Delete/{id}
        [HttpDelete("delete/{id:int}")] // route: /api/Rooms/Delete/{id}
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var existingRoom = await roomRepository.GetRoomByIdAsync(id);
                if (existingRoom.RoomId <= 0)
                {
                    return NotFound($"Room with ID {id} does not exist.");
                }

                var success = await roomRepository.DeleteRoomAsync(id);
                if (!success)
                {
                    return BadRequest("Failed to delete room.");
                }

                return Ok($"Room with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteRoom: An error occurred while deleting the room.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
