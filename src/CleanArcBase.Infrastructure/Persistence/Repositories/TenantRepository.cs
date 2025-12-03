using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Domain.Entities.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace CleanArcBase.Infrastructure.Persistence.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.Identifier == identifier, cancellationToken);
    }

    public async Task<bool> IsIdentifierUniqueAsync(string identifier, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(t => t.Identifier == identifier);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }
}
