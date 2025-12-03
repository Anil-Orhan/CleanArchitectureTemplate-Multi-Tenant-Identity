using CleanArcBase.Domain.Entities.Tenancy;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace CleanArcBase.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public Guid? TenantId { get; private set; }
    public Tenant? Tenant { get; private set; }

    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private ApplicationRole() { }

    public ApplicationRole(string name, string? description = null, bool isSystemRole = false, Guid? tenantId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Id = NewId.NextGuid();
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        IsSystemRole = isSystemRole;
        TenantId = tenantId;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public void AssignPermission(Guid permissionId)
    {
        if (_rolePermissions.All(rp => rp.PermissionId != permissionId))
        {
            _rolePermissions.Add(new RolePermission(Id, permissionId));
        }
    }

    public void RemovePermission(Guid permissionId)
    {
        var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            _rolePermissions.Remove(rolePermission);
        }
    }

    public void ClearPermissions()
    {
        _rolePermissions.Clear();
    }
}
