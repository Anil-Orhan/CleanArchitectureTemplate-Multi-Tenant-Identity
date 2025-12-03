namespace CleanArcBase.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    Guid? TenantId { get; }
    string? TenantIdentifier { get; }
    void SetTenant(Guid tenantId, string identifier);
}
