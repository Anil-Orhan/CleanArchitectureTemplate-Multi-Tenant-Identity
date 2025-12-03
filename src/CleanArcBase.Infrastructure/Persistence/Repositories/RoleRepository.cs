using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArcBase.Infrastructure.Persistence.Repositories;

public class RoleRepository : Repository<ApplicationRole>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ApplicationRole?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationRole>> GetByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.TenantId == null || r.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationRole?> GetByNameAsync(string name, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => r.Name == name && (r.TenantId == null || r.TenantId == tenantId), cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? tenantId = null, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(r => r.Name == name && (r.TenantId == null || r.TenantId == tenantId));

        if (excludeId.HasValue)
            query = query.Where(r => r.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }
}
