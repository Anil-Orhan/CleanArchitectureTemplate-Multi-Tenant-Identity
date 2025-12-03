using CleanArcBase.Application.Common.Interfaces.Repositories;

namespace CleanArcBase.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    ITenantRepository Tenants { get; }
    IPermissionRepository Permissions { get; }
    IRoleRepository Roles { get; }
    IRoleGroupRepository RoleGroups { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
