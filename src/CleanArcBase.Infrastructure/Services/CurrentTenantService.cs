using CleanArcBase.Application.Common.Interfaces;

namespace CleanArcBase.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid? TenantId { get; private set; }
    public string? TenantIdentifier { get; private set; }

    public void SetTenant(Guid tenantId, string identifier)
    {
        TenantId = tenantId;
        TenantIdentifier = identifier;
    }
}
