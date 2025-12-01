using medibook_API.Data;
using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<RoomRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly ILogRepository logRepository;

        public RoomRepository(
            Medibook_Context database,
            ILogger<RoomRepository> logger,
            StringNormalizer stringNormalizer,
            ILogRepository logRepository)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
            this.logRepository = logRepository;
        }
        public async Task<bool> ActiveRoomAsync(int roomId)
        {
            try
            {
                var room = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == roomId);

                if (room == null)
                {
                    string message = $"Room with ID {roomId} not found.";
                    logger.LogWarning(message);
                    await logRepository.CreateLogAsync("Activate Room", "Warning", message);
                    return false;
                }

                room.is_active = true;
                database.Rooms.Update(room);
                await database.SaveChangesAsync();

                string successMsg = $"Room with ID {roomId} activated successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Activate Room", "Success", successMsg);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while activating room.");
                await logRepository.CreateLogAsync("Activate Room", "Error", ex.Message);
                return false;
            }
        }
        public async Task<RoomDetailsDto> CreateRoomAsync(CreateRoomDto dto)
        {
            try
            {
                if (dto == null)
                {
                    string msg = "CreateRoomAsync: Room object is null.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Create Room", "Warning", msg);
                    return new RoomDetailsDto();
                }
                var room = new Rooms();
                room.room_name = stringNormalizer.NormalizeName(dto.RoomName);
                room.room_type = stringNormalizer.NormalizeName(dto.RoomType);
                room.is_active = true;
                room.create_date = DateTime.Now;

                await database.Rooms.AddAsync(room);
                await database.SaveChangesAsync();

                string successMsg = $"Room '{room.room_name}' of type '{room.room_type}' created successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Create Room", "Success", successMsg);

                return MapingRooms(room);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating room.");
                await logRepository.CreateLogAsync("Create Room", "Error", ex.Message);
                return new RoomDetailsDto();
            }
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            try
            {
                var room = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == roomId);

                if (room == null)
                {
                    string msg = $"Room with ID {roomId} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Delete Room", "Warning", msg);
                    return false;
                }

                database.Rooms.Remove(room);
                await database.SaveChangesAsync();

                string successMsg = $"Room with ID {roomId} deleted successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Delete Room", "Success", successMsg);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting room.");
                await logRepository.CreateLogAsync("Delete Room", "Error", ex.Message);
                return false;
            }
        }
        public async Task<IEnumerable<RoomDetailsDto>> GetALlActiveAsync()
        {
            try
            {
                var list = await database.Rooms
                    .Where(r => r.is_active)
                    .OrderBy(r => r.room_name)
                    .ToListAsync();


                await logRepository.CreateLogAsync("Get Active Rooms", "Success", "Active rooms retrieved successfully.");

                return list.Select(MapingRooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving active rooms.");
                await logRepository.CreateLogAsync("Get Active Rooms", "Error", ex.Message);
                return Enumerable.Empty<RoomDetailsDto>();
            }
        }
        public async Task<IEnumerable<RoomDetailsDto>> GetALlActiveAsync(DateTime appointmentDate)
        {
            try
            {
                // Get the start and end of the day for the appointment date
                var startOfDay = appointmentDate.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                // Get all room IDs that have appointments on this date
                var roomsWithAppointments = await database.Appointments
                    .Where(a => a.room_id.HasValue && 
                                a.appointment_date >= startOfDay && 
                                a.appointment_date <= endOfDay)
                    .Select(a => a.room_id.Value)
                    .Distinct()
                    .ToListAsync();

                // Get active rooms that don't have appointments on this date
                var list = await database.Rooms
                    .Where(r => r.is_active && !roomsWithAppointments.Contains(r.room_id))
                    .OrderBy(r => r.room_name)
                    .ToListAsync();

                string msg = $"Fetched {list.Count} active rooms without appointments on {appointmentDate:yyyy-MM-dd}.";
                logger.LogInformation(msg);
                await logRepository.CreateLogAsync("Get Active Rooms By Date", "Success", msg);

                return list.Select(MapingRooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving active rooms by date.");
                await logRepository.CreateLogAsync("Get Active Rooms By Date", "Error", ex.Message);
                return Enumerable.Empty<RoomDetailsDto>();
            }
        }
        public async Task<IEnumerable<RoomDetailsDto>> GetALlRoomsAsync()
        {
            try
            {
                var list = await database.Rooms
                    .OrderBy(r => r.room_name)
                    .ToListAsync();

                await logRepository.CreateLogAsync("Get Rooms", "Success", "Rooms retrieved successfully.");

                return list.Select(MapingRooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving rooms.");
                await logRepository.CreateLogAsync("Get Rooms", "Error", ex.Message);
                return Enumerable.Empty<RoomDetailsDto>();
            }
        }
        public async Task<RoomDetailsDto> GetRoomByIdAsync(int id)
        {
            try
            {
                var room = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == id);

                if (room == null)
                {
                    string msg = $"Room with ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Get Room By ID", "Warning", msg);
                    return new RoomDetailsDto();
                }

                await logRepository.CreateLogAsync("Get Room By ID", "Success", $"Room with ID {id} retrieved.");
                return MapingRooms(room);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving room by ID.");
                await logRepository.CreateLogAsync("Get Room By ID", "Error", ex.Message);
                return new RoomDetailsDto();
            }
        }
        public async Task<bool> InactiveRoomAsync(int roomId)
        {
            try
            {
                var room = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == roomId);

                if (room == null)
                {
                    string msg = $"Room with ID {roomId} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Inactive Room", "Warning", msg);
                    return false;
                }

                room.is_active = false;
                database.Rooms.Update(room);
                await database.SaveChangesAsync();

                string successMsg = $"Room with ID {roomId} deactivated successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Inactive Room", "Success", successMsg);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deactivating room.");
                await logRepository.CreateLogAsync("Inactive Room", "Error", ex.Message);
                return false;
            }
        }
        public async Task<bool> IsRoomExist(string name, string type, int id)
        {
            try
            {
                var normalizedName = stringNormalizer.NormalizeName(name);
                var normalizedType = stringNormalizer.NormalizeName(type);

                var query = database.Rooms
                    .Where(r => r.room_name == normalizedName &&
                                r.room_type == normalizedType);

                if (id != 0)
                    query = query.Where(r => r.room_id != id);

                bool exists = await query.AnyAsync();

                await logRepository.CreateLogAsync("Check Room Exists", "Success",
                    $"Room exist check: Name={name}, Type={type}, Exists={exists}");

                return exists;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking room existence.");
                await logRepository.CreateLogAsync("Check Room Exists", "Error", ex.Message);
                return false;
            }
        }
        public async Task<RoomDetailsDto> UpdateRoomAsync(RoomDetailsDto room)
        {
            try
            {
                var existingRoom = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == room.RoomId);

                if (existingRoom == null)
                {
                    string msg = $"Room with ID {room.RoomId} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Update Room", "Warning", msg);
                    return new RoomDetailsDto();
                }

                existingRoom.room_name = stringNormalizer.NormalizeName(room.RoomName);
                existingRoom.room_type = stringNormalizer.NormalizeName(room.RoomType);
                existingRoom.is_active = room.IsActive;
                existingRoom.create_date = room.CreateDate;

                database.Rooms.Update(existingRoom);
                await database.SaveChangesAsync();

                string successMsg = $"Room with ID {room.RoomId} updated successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Update Room", "Success", successMsg);

                return MapingRooms(existingRoom);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating room.");
                await logRepository.CreateLogAsync("Update Room", "Error", ex.Message);
                return new RoomDetailsDto();
            }
        }
        private RoomDetailsDto MapingRooms(Rooms r)
        {
            return new RoomDetailsDto
            {
                RoomId = r.room_id,
                RoomName = r.room_name,
                RoomType = r.room_type,
                IsActive = r.is_active,
                CreateDate = r.create_date,

            };
        }
    }
}
