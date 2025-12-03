using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Application.Common.Interfaces.Repositories;

public interface IRoleGroupRepository : IRepository<RoleGroup>
{
    Task<RoleGroup?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleGroup>> GetByTenantWithRolesAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleGroup>> GetSystemGroupsWithRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, Guid? tenantId, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
