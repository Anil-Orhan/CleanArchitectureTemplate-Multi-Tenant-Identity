using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Domain.Entities.Identity;
using CleanArcBase.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArcBase.Infrastructure.Persistence.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Permission>> GetByGroupAsync(PermissionGroup group, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(p => p.Group == group).ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
    }
}
