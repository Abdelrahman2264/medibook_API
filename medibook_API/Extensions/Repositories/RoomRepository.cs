using medibook_API.Data;
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
        public async Task<Rooms> CreateRoomAsync(Rooms room)
        {
            try
            {
                if (room == null)
                {
                    string msg = "CreateRoomAsync: Room object is null.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Create Room", "Warning", msg);
                    return new Rooms();
                }

                room.room_name = stringNormalizer.NormalizeName(room.room_name);
                room.room_type = stringNormalizer.NormalizeName(room.room_type);
                room.is_active = true;
                room.create_date = DateTime.Now;

                await database.Rooms.AddAsync(room);
                await database.SaveChangesAsync();

                string successMsg = $"Room '{room.room_name}' of type '{room.room_type}' created successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Create Room", "Success", successMsg);

                return room;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating room.");
                await logRepository.CreateLogAsync("Create Room", "Error", ex.Message);
                return new Rooms();
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
        public async Task<IEnumerable<Rooms>> GetALlActiveAsync()
        {
            try
            {
                var list = await database.Rooms
                    .Where(r => r.is_active)
                    .OrderBy(r => r.room_name)
                    .ToListAsync();

                await logRepository.CreateLogAsync("Get Active Rooms", "Success", "Active rooms retrieved successfully.");

                return list;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving active rooms.");
                await logRepository.CreateLogAsync("Get Active Rooms", "Error", ex.Message);
                return Enumerable.Empty<Rooms>();
            }
        }
        public async Task<IEnumerable<Rooms>> GetALlRoomsAsync()
        {
            try
            {
                var list = await database.Rooms
                    .OrderBy(r => r.room_name)
                    .ToListAsync();

                await logRepository.CreateLogAsync("Get Rooms", "Success", "Rooms retrieved successfully.");

                return list;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving rooms.");
                await logRepository.CreateLogAsync("Get Rooms", "Error", ex.Message);
                return Enumerable.Empty<Rooms>();
            }
        }
        public async Task<Rooms> GetRoomByIdAsync(int id)
        {
            try
            {
                var room = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == id);

                if (room == null)
                {
                    string msg = $"Room with ID {id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Get Room By ID", "Warning", msg);
                    return new Rooms();
                }

                await logRepository.CreateLogAsync("Get Room By ID", "Success", $"Room with ID {id} retrieved.");
                return room;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving room by ID.");
                await logRepository.CreateLogAsync("Get Room By ID", "Error", ex.Message);
                return new Rooms();
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
        public async Task<Rooms> UpdateRoomAsync(Rooms room)
        {
            try
            {
                var existingRoom = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == room.room_id);

                if (existingRoom == null)
                {
                    string msg = $"Room with ID {room.room_id} not found.";
                    logger.LogWarning(msg);
                    await logRepository.CreateLogAsync("Update Room", "Warning", msg);
                    return new Rooms();
                }

                existingRoom.room_name = stringNormalizer.NormalizeName(room.room_name);
                existingRoom.room_type = stringNormalizer.NormalizeName(room.room_type);
                existingRoom.is_active = room.is_active;
                existingRoom.create_date = room.create_date;

                database.Rooms.Update(existingRoom);
                await database.SaveChangesAsync();

                string successMsg = $"Room with ID {room.room_id} updated successfully.";
                logger.LogInformation(successMsg);
                await logRepository.CreateLogAsync("Update Room", "Success", successMsg);

                return existingRoom;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating room.");
                await logRepository.CreateLogAsync("Update Room", "Error", ex.Message);
                return new Rooms();
            }
        }
    }
}
