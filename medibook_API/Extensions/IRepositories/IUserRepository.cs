using medibook_API.Extensions.DTOs;

namespace medibook_API.Extensions.IRepositories
{
    public interface IUserRepository
    {

        public Task<bool> InActiveUserAsync(int id);
        public Task<bool> ActiveUserAsync(int id);
        public Task<bool> IsEmailExistAsync(string email, int id);
        public Task<bool> IsPhoneExistAsync(string phone, int id);
        public Task<UserDetailsDto> GetUserByIdAsync(int id);
        public Task<IEnumerable<UserDetailsDto>> GetAllUsersAsync();
        public Task<IEnumerable<UserDetailsDto>> GetAllActiveUsersAsync();

        public Task<CreatedResponseDto> CreateUserAsync(CreateUserDto dto);
        public Task<CreatedResponseDto> CreateAdminAsync(CreateUserDto dto);
        public Task<IEnumerable<UserDetailsDto>> GetAllAdminsAsync();
        public Task<IEnumerable<UserDetailsDto>> GetAllActiveAdminsAsync();

        public Task<UserDetailsDto> UpdateUserAsync(UpdateUserDto dto , int id);


    }
}
