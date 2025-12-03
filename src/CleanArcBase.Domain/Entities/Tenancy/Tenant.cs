using CleanArcBase.Domain.Common;
using CleanArcBase.Domain.Entities.Identity;
using MassTransit;

namespace CleanArcBase.Domain.Entities.Tenancy;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Identifier { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public string? Settings { get; private set; }

    private readonly List<RoleGroup> _roleGroups = new();
    public IReadOnlyCollection<RoleGroup> RoleGroups => _roleGroups.AsReadOnly();

    private Tenant() { }

    public Tenant(string name, string identifier)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Tenant identifier cannot be empty", nameof(identifier));

        Name = name;
        Identifier = identifier.ToLowerInvariant();
        IsActive = true;
    }

    public void Update(string name, string? settings = null)
    {
        Name = name;
        Settings = settings;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
