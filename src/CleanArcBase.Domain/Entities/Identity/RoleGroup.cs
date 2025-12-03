using CleanArcBase.Domain.Common;
using CleanArcBase.Domain.Entities.Tenancy;

namespace CleanArcBase.Domain.Entities.Identity;

public class RoleGroup : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }
    public int DisplayOrder { get; private set; }

    public Guid? TenantId { get; private set; }
    public Tenant? Tenant { get; private set; }

    public Guid? SourceRoleGroupId { get; private set; }
    public RoleGroup? SourceRoleGroup { get; private set; }

    private readonly List<RoleGroupRole> _roleGroupRoles = new();
    public IReadOnlyCollection<RoleGroupRole> RoleGroupRoles => _roleGroupRoles.AsReadOnly();

    private RoleGroup() { }

    public RoleGroup(string name, string? description = null, bool isSystem = false,
                     int displayOrder = 0, Guid? tenantId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role group name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        IsSystem = isSystem;
        DisplayOrder = displayOrder;
        TenantId = tenantId;
    }

    public void Update(string name, string? description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role group name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
    }

    public RoleGroup Clone(Guid tenantId)
    {
        var cloned = new RoleGroup(Name, Description, isSystem: false, DisplayOrder, tenantId);
        cloned.SourceRoleGroupId = this.Id;
        return cloned;
    }

    public void AssignRole(Guid roleId)
    {
        if (_roleGroupRoles.All(rgr => rgr.RoleId != roleId))
        {
            _roleGroupRoles.Add(new RoleGroupRole(Id, roleId));
        }
    }

    public void RemoveRole(Guid roleId)
    {
        var item = _roleGroupRoles.FirstOrDefault(rgr => rgr.RoleId == roleId);
        if (item != null)
        {
            _roleGroupRoles.Remove(item);
        }
    }

    public void ClearRoles()
    {
        _roleGroupRoles.Clear();
    }

    public void SetRoles(IEnumerable<Guid> roleIds)
    {
        _roleGroupRoles.Clear();
        foreach (var roleId in roleIds)
        {
            _roleGroupRoles.Add(new RoleGroupRole(Id, roleId));
        }
    }
}
