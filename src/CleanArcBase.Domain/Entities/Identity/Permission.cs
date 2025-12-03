using CleanArcBase.Domain.Enums;
using MassTransit;

namespace CleanArcBase.Domain.Entities.Identity;

public class Permission
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public PermissionGroup Group { get; private set; }
    public string? Description { get; private set; }

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Permission() { }

    public Permission(string name, string displayName, PermissionGroup group, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Permission display name cannot be empty", nameof(displayName));

        Id = NewId.NextGuid();
        Name = name;
        DisplayName = displayName;
        Group = group;
        Description = description;
    }
}
