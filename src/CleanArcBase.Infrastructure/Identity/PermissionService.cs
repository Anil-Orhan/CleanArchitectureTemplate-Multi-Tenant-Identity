using Microsoft.EntityFrameworkCore;
using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Infrastructure.Persistence;

namespace CleanArcBase.Infrastructure.Identity;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;

    public PermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Single query to get all permissions for a user through their roles
        var permissions = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == userId
            select p.Name
        ).Distinct().ToListAsync(cancellationToken);

        return permissions;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permission);
    }
}
