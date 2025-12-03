namespace CleanArcBase.Application.Features.Permissions;

public class PermissionItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Group { get; set; } = null!;
    public string? Description { get; set; }
}

public class PermissionGroupDto
{
    public string Group { get; set; } = null!;
    public List<PermissionItemDto> Permissions { get; set; } = new();
}
