namespace CleanArcBase.Application.Features.Roles;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public Guid? TenantId { get; set; }
}

public class RoleDetailDto : RoleDto
{
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}
