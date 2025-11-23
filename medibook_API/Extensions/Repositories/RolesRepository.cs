using medibook_API.Data;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;

namespace medibook_API.Extensions.Repositories
{
    public class RolesRepository : IRolesRepository
    {

        private readonly Medibook_Context database; 
        private readonly ILogger<RolesRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        public RolesRepository(Medibook_Context database, ILogger<RolesRepository> logger, StringNormalizer stringNormalizer)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
        }

        public async Task<Roles> CreateNewRoleAsync(Roles role)
        {

            try
            {
                if (role == null)
                {
                    logger.LogWarning("Role object is null");
                    return new Roles();
                }
                role.role_name = stringNormalizer.NormalizeName(role.role_name);
                role.create_date = DateTime.Now;
                await database.Roles.AddAsync(role);
                await database.SaveChangesAsync();
                return role;

            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while creating a new role: {Message}", ex.Message);
                return new Roles();

            }
        }

        public async Task<IEnumerable<Roles>> GetAllRolesAsync()
        {
            try
            {
                var roles = await database.Roles.ToListAsync();
                return roles;

            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while retrieving all roles: {Message}", ex.Message);
                return Enumerable.Empty<Roles>();
            }

        }

        public async Task<Roles> GetRoleByIdAsync(int roleId)
        {
            try
            {
                var role = await database.Roles.FirstOrDefaultAsync(u => u.role_id == roleId);
                if (role == null)
                {
                    logger.LogWarning("Role with ID {RoleId} not found", roleId);
                    return new Roles();
                }
                return role;

            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while retrieving specific role with Id {roleId}: {ex.Message}");
                return new Roles();
            }
        }

        public async Task<bool> IsRoleExistAsync(string roleName)
        {
            try
            {
                roleName = stringNormalizer.NormalizeName(roleName);
                var role = await database.Roles.FirstOrDefaultAsync(r => r.role_name == roleName);
                if (role == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while checking if role exists: {Message}", ex.Message);
                return false;
            }
        }
    }
}
