using CleanArcBase.Application.Features.Roles;

namespace CleanArcBase.Application.Features.RoleGroups;

public class RoleGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? SourceRoleGroupId { get; set; }
}

public class RoleGroupDetailDto : RoleGroupDto
{
    public List<RoleDto> Roles { get; set; } = new();
}
