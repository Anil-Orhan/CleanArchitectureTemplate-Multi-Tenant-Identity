using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArcBase.Infrastructure.Persistence.Repositories;

public class RoleGroupRepository : Repository<RoleGroup>, IRoleGroupRepository
{
    public RoleGroupRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RoleGroup?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(rg => rg.RoleGroupRoles)
                .ThenInclude(rgr => rgr.Role)
            .FirstOrDefaultAsync(rg => rg.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<RoleGroup>> GetByTenantWithRolesAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(rg => rg.RoleGroupRoles)
                .ThenInclude(rgr => rgr.Role)
            .Where(rg => rg.TenantId == null || rg.TenantId == tenantId)
            .OrderBy(rg => rg.DisplayOrder)
            .ThenBy(rg => rg.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoleGroup>> GetSystemGroupsWithRolesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .IgnoreQueryFilters()
            .Include(rg => rg.RoleGroupRoles)
                .ThenInclude(rgr => rgr.Role)
            .Where(rg => rg.TenantId == null && rg.IsSystem)
            .OrderBy(rg => rg.DisplayOrder)
            .ThenBy(rg => rg.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? tenantId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(rg => rg.Name == name && rg.TenantId == tenantId);

        if (excludeId.HasValue)
            query = query.Where(rg => rg.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }
}
