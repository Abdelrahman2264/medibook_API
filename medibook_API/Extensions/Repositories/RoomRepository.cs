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

        public RoomRepository(Medibook_Context database,
            ILogger<RoomRepository> logger,
            StringNormalizer stringNormalizer)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
        }


        public async Task<bool> ActiveRoomAsync(int roomId)
        {
            try
            {
                var room = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == roomId);
                if (room == null)
                {
                    logger.LogWarning("ActiveRoomAsync: Room with ID {RoomId} not found.", roomId);
                    return false;
                }
                room.is_active = true;
                database.Rooms.Update(room);
                await database.SaveChangesAsync();
                logger.LogInformation("ActiveRoomAsync: Room with ID {RoomId} Activated successfully.", roomId);
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ActiveRoomAsync: An error occurred while Activating the room.");
                return false;
            }
        }

        public async Task<Rooms> CreateRoomAsync(Rooms room)
        {
            try
            {
                if (room == null)
                {
                    logger.LogWarning("CreateRoomAsync: room parameter is null.");
                    return new Rooms();
                }
                room.room_name = stringNormalizer.NormalizeName(room.room_name);
                room.room_type = stringNormalizer.NormalizeName(room.room_type);
                room.is_active = true;
                room.create_date = DateTime.Now;
                await database.Rooms.AddAsync(room);
                await database.SaveChangesAsync();
                logger.LogInformation("CreateRoomAsync: Room created successfully with ID {RoomId}.", room.room_id);
                return room;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateRoomAsync: An error occurred while creating the room.");
                return new Rooms();
            }
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            try
            {
                if (roomId <= 0)
                {
                    logger.LogWarning("Room Id Is Not Vaild");
                    return false;
                }
                var room = await database.Rooms.FirstOrDefaultAsync(u => u.room_id == roomId);
                if (room == null)
                {
                    logger.LogWarning($"Room with id {roomId} is not found");
                    return false;
                }
                database.Rooms.Remove(room);
                await database.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError("Error while delete a room {ex}", ex);
                return false;


            }
        }

        public async Task<IEnumerable<Rooms>> GetALlActiveAsync()
        {
            try
            {
                return await database.Rooms
                    .Where(r => r.is_active)
                    .OrderBy(r => r.room_name)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetALlActiveAsync: An error occurred while retrieving active rooms.");
                return Enumerable.Empty<Rooms>();
            }

        }

        public async Task<IEnumerable<Rooms>> GetALlRoomsAsync()
        {
            try
            {
                return await database.Rooms
                    .OrderBy(r => r.room_name)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllRomsAsync: An error occurred while retrieving rooms.");
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
                    logger.LogWarning("GetRoomByIdAsync: Room with ID {RoomId} not found.", id);
                    return new Rooms();
                }
                return room;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRoomByIdAsync: An error occurred while retrieving the room by ID.");
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
                    logger.LogWarning("InactiveRoomAsync: Room with ID {RoomId} not found.", roomId);
                    return false;
                }
                room.is_active = false;
                database.Rooms.Update(room);
                await database.SaveChangesAsync();
                logger.LogInformation("InactiveRoomAsync: Room with ID {RoomId} deactivated successfully.", roomId);
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "InactiveRoomAsync: An error occurred while deactivating the room.");
                return false;
            }
        }

      
        public async Task<bool> IsRoomExist(string name, string type, int id)
        {
            try
            {
                var normalizedName = stringNormalizer.NormalizeName(name);
                var normalizedType = stringNormalizer.NormalizeName(type);

                IQueryable<Rooms> query = database.Rooms
                    .Where(r => r.room_name == normalizedName &&
                                r.room_type == normalizedType);
                if (id != 0)
                {
                    query = query.Where(r => r.room_id != id);
                }

                var room = await query.FirstOrDefaultAsync();

                bool exists = room != null;

                logger.LogInformation(
                    "IsRoomExist: Room with name {RoomName} and type {RoomType} existence: {Exists}",
                    name, type, exists
                );

                return exists;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IsRoomExist: An error occurred while checking if the room exists.");
                return false;
            }
        }


        public async Task<Rooms> UpdateRoomAsync(Rooms room)
        {
            try
            {
                if (room == null)
                {
                    logger.LogWarning("UpdateRoomAsync: room parameter is null.");
                    return new Rooms();
                }
                var existingRoom = await database.Rooms.FirstOrDefaultAsync(r => r.room_id == room.room_id);
                if (existingRoom == null)
                {
                    logger.LogWarning("UpdateRoomAsync: Room with ID {RoomId} not found.", room.room_id);
                    return new Rooms();
                }
                existingRoom.room_name = stringNormalizer.NormalizeName(room.room_name);
                existingRoom.room_type = stringNormalizer.NormalizeName(room.room_type);
                existingRoom.is_active = room.is_active;
                existingRoom.create_date = room.create_date;
                database.Rooms.Update(existingRoom);
                await database.SaveChangesAsync();
                logger.LogInformation("UpdateRoomAsync: Room with ID {RoomId} updated successfully.", room.room_id);
                return existingRoom;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateRoomAsync: An error occurred while updating the room.");
                return new Rooms();
            }
        }
    }
}
