using CleanArcBase.Domain.Entities.Identity;
using CleanArcBase.Domain.Enums;

namespace CleanArcBase.Application.Common.Interfaces.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<IReadOnlyList<Permission>> GetByGroupAsync(PermissionGroup group, CancellationToken cancellationToken = default);
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
