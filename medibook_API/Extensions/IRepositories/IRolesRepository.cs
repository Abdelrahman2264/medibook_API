using medibook_API.Models;

namespace medibook_API.Extensions.IRepositories
{
    public interface IRolesRepository
    {
        public Task<bool> IsRoleExistAsync(string roleName);
        public Task<Roles> CreateNewRoleAsync(Roles role);
        public Task<Roles> GetRoleByIdAsync(int roleId);
        public Task<IEnumerable<Roles>> GetAllRolesAsync();
        

    }
}
