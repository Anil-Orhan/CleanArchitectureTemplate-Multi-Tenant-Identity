using CleanArcBase.Domain.Entities.Tenancy;

namespace CleanArcBase.Application.Common.Interfaces.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<bool> IsIdentifierUniqueAsync(string identifier, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
