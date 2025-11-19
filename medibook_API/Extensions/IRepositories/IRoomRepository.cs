using medibook_API.Models;

namespace medibook_API.Extensions.IRepositories
{
    public interface IRoomRepository
    {
        public Task<Rooms> CreateRoomAsync(Rooms room);
        public Task<Rooms> UpdateRoomAsync(Rooms room);
        public Task<Rooms> GetRoomByIdAsync(int id);
        public Task<bool> ActiveRoomAsync(int roomId);
        public Task<bool> InactiveRoomAsync(int roomId);
        public Task<bool> DeleteRoomAsync(int roomId);
        public Task<bool> IsRoomExist(string name, string type, int id);
        public Task<IEnumerable<Rooms>> GetALlRoomsAsync();
        public Task<IEnumerable<Rooms>> GetALlActiveAsync();


    }
}
