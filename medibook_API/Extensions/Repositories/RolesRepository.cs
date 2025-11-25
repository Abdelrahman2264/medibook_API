using medibook_API.Data;
using medibook_API.Extensions.IRepositories;
using medibook_API.Extensions.Services;
using medibook_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace medibook_API.Extensions.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly Medibook_Context database;
        private readonly ILogger<RolesRepository> logger;
        private readonly StringNormalizer stringNormalizer;
        private readonly IHttpContextAccessor _context;
        private readonly ILogRepository logRepository;

        public RolesRepository(
            Medibook_Context database,
            ILogger<RolesRepository> logger,
            StringNormalizer stringNormalizer,
            IHttpContextAccessor context,
            ILogRepository logRepository)
        {
            this.database = database;
            this.logger = logger;
            this.stringNormalizer = stringNormalizer;
            _context = context;
            this.logRepository = logRepository;
        }

        public int GetCurrentUserId()
        {
            var id = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(id))
                return 0;

            return int.TryParse(id, out int userId) ? userId : 0;
        }

        private async Task<string> GetCurrentUserNameSafeAsync()
        {
            try
            {
                var user = await GetCurrentUserData(GetCurrentUserId());
                return user == null
                    ? "Unknown User"
                    : $"{user.first_name} {user.last_name}";
            }
            catch
            {
                return "Unknown User";
            }
        }

        public async Task<Users> GetCurrentUserData(int userid)
        {
            try
            {
                var user = await database.Users
                    .Include(r => r.Role)
                    .FirstOrDefaultAsync(u => u.user_id == userid);

                if (user == null)
                {
                    logger.LogWarning("User with ID {UserId} not found", userid);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving current user data");
                return null;
            }
        }

        public async Task<Roles> CreateNewRoleAsync(Roles role)
        {
            string userName = await GetCurrentUserNameSafeAsync();

            try
            {
                if (role == null)
                {
                    logger.LogWarning("Role object is null");
                    await logRepository.CreateLogAsync("Create Role", "Warning", $"Role object was null. User: {userName}");
                    return new Roles();
                }

                role.role_name = stringNormalizer.NormalizeName(role.role_name);
                role.create_date = DateTime.Now;

                await database.Roles.AddAsync(role);
                await database.SaveChangesAsync();

                await logRepository.CreateLogAsync(
                    "Create Role",
                    "Success",
                    $"Role '{role.role_name}' created by {userName}."
                );

                return role;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating a new role");

                await logRepository.CreateLogAsync(
                    "Create Role",
                    "Error",
                    $"Error creating role '{role?.role_name}' by {userName}. Exception: {ex.Message}"
                );

                return new Roles();
            }
        }

        public async Task<IEnumerable<Roles>> GetAllRolesAsync()
        {
            string userName = await GetCurrentUserNameSafeAsync();

            try
            {
                var roles = await database.Roles.ToListAsync();

                await logRepository.CreateLogAsync(
                    "Get All Roles",
                    "Success",
                    $"All roles retrieved by {userName}."
                );

                return roles;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all roles");

                await logRepository.CreateLogAsync(
                    "Get All Roles",
                    "Error",
                    $"Error retrieving all roles by {userName}. Exception: {ex.Message}"
                );

                return Enumerable.Empty<Roles>();
            }
        }

        public async Task<Roles> GetRoleByIdAsync(int roleId)
        {
            string userName = await GetCurrentUserNameSafeAsync();

            try
            {
                var role = await database.Roles.FirstOrDefaultAsync(r => r.role_id == roleId);

                if (role == null)
                {
                    logger.LogWarning("Role with ID {RoleId} not found", roleId);

                    await logRepository.CreateLogAsync(
                        "Get Role By Id",
                        "Warning",
                        $"Role with ID {roleId} not found. Requested by {userName}."
                    );

                    return new Roles();
                }

                await logRepository.CreateLogAsync(
                    "Get Role By Id",
                    "Success",
                    $"Role '{role.role_name}' retrieved by {userName}."
                );

                return role;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving role with ID {RoleId}", roleId);

                await logRepository.CreateLogAsync(
                    "Get Role By Id",
                    "Error",
                    $"Error retrieving role with ID {roleId} by {userName}. Exception: {ex.Message}"
                );

                return new Roles();
            }
        }

        public async Task<bool> IsRoleExistAsync(string roleName)
        {
            try
            {
                roleName = stringNormalizer.NormalizeName(roleName);

                var role = await database.Roles.FirstOrDefaultAsync(r => r.role_name == roleName);

                return role != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking if role exists");

                await logRepository.CreateLogAsync(
                    "Is Role Exists",
                    "Error",
                    $"Error while checking if role '{roleName}' exists. Exception: {ex.Message}"
                );

                return false;
            }
        }
    }
}
