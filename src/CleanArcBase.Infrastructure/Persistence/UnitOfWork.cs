using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArcBase.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private ITenantRepository? _tenants;
    private IPermissionRepository? _permissions;
    private IRoleRepository? _roles;
    private IRoleGroupRepository? _roleGroups;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ITenantRepository Tenants => _tenants ??= new TenantRepository(_context);
    public IPermissionRepository Permissions => _permissions ??= new PermissionRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IRoleGroupRepository RoleGroups => _roleGroups ??= new RoleGroupRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
