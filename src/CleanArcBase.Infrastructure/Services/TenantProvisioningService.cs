using CleanArcBase.Application.Common.Interfaces;

namespace CleanArcBase.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly IUnitOfWork _unitOfWork;

    public TenantProvisioningService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ProvisionTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // 1. Get system RoleGroups (TenantId = null, IsSystem = true)
        var systemGroups = await _unitOfWork.RoleGroups
            .GetSystemGroupsWithRolesAsync(cancellationToken);

        // 2. Clone each group to the tenant
        foreach (var systemGroup in systemGroups)
        {
            var tenantGroup = systemGroup.Clone(tenantId);

            // 3. Copy role assignments from the system group
            foreach (var roleGroupRole in systemGroup.RoleGroupRoles)
            {
                tenantGroup.AssignRole(roleGroupRole.RoleId);
            }

            await _unitOfWork.RoleGroups.AddAsync(tenantGroup, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
