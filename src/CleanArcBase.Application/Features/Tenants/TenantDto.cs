namespace CleanArcBase.Application.Features.Tenants;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Identifier { get; set; } = null!;
    public bool IsActive { get; set; }
}
