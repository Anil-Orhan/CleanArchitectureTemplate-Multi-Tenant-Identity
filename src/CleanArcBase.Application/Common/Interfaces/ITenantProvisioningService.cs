namespace CleanArcBase.Application.Common.Interfaces;

public interface ITenantProvisioningService
{
    Task ProvisionTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
