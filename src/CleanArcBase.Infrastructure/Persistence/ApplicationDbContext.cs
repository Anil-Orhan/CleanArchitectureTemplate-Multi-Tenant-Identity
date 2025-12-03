using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Domain.Common;
using CleanArcBase.Domain.Entities.Identity;
using CleanArcBase.Domain.Entities.Tenancy;

namespace CleanArcBase.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IDomainEventDispatcher? _domainEventDispatcher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        ICurrentTenantService currentTenantService,
        IDateTimeService dateTimeService,
        IDomainEventDispatcher? domainEventDispatcher = null)
        : base(options)
    {
        _currentUserService = currentUserService;
        _currentTenantService = currentTenantService;
        _dateTimeService = dateTimeService;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<RoleGroup> RoleGroups => Set<RoleGroup>();
    public DbSet<RoleGroupRole> RoleGroupRoles => Set<RoleGroupRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for multi-tenancy
        modelBuilder.Entity<ApplicationUser>()
            .HasQueryFilter(u => _currentTenantService.TenantId == null || u.TenantId == _currentTenantService.TenantId);

        // RoleGroup filter - show system groups (TenantId=null) OR tenant's own groups
        modelBuilder.Entity<RoleGroup>()
            .HasQueryFilter(rg => rg.TenantId == null ||
                                  _currentTenantService.TenantId == null ||
                                  rg.TenantId == _currentTenantService.TenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedInfo(_currentUserService.UserId, _dateTimeService.UtcNow);
                    break;

                case EntityState.Modified:
                    entry.Entity.SetUpdatedInfo(_currentUserService.UserId, _dateTimeService.UtcNow);
                    break;
            }
        }

        // Collect entities with domain events before saving
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after successful save
        if (_domainEventDispatcher != null && entitiesWithEvents.Any())
        {
            await _domainEventDispatcher.DispatchEventsAsync(entitiesWithEvents, cancellationToken);
        }

        return result;
    }
}
