using medibook_API.Extensions.DTOs;
using medibook_API.Models;

namespace medibook_API.Extensions.IRepositories
{
    public interface IRoomRepository
    {
        public Task<RoomDetailsDto> CreateRoomAsync(CreateRoomDto room);
        public Task<RoomDetailsDto> UpdateRoomAsync(RoomDetailsDto room);
        public Task<RoomDetailsDto> GetRoomByIdAsync(int id);
        public Task<bool> ActiveRoomAsync(int roomId);
        public Task<bool> InactiveRoomAsync(int roomId);
        public Task<bool> DeleteRoomAsync(int roomId);
        public Task<bool> IsRoomExist(string name, string type, int id);
        public Task<IEnumerable<RoomDetailsDto>> GetALlRoomsAsync();
        public Task<IEnumerable<RoomDetailsDto>> GetALlActiveAsync();
        public Task<IEnumerable<RoomDetailsDto>> GetALlActiveAsync(DateTime appointmentDate);


    }
}
