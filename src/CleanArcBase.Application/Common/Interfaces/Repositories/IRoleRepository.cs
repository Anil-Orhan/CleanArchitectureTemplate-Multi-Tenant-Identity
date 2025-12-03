using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Application.Common.Interfaces.Repositories;

public interface IRoleRepository : IRepository<ApplicationRole>
{
    Task<ApplicationRole?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApplicationRole>> GetByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<ApplicationRole?> GetByNameAsync(string name, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, Guid? tenantId = null, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
