namespace CleanArcBase.Domain.Entities.Identity;

public class RoleGroupRole
{
    public Guid RoleGroupId { get; private set; }
    public RoleGroup RoleGroup { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public ApplicationRole Role { get; private set; } = null!;

    private RoleGroupRole() { }

    public RoleGroupRole(Guid roleGroupId, Guid roleId)
    {
        RoleGroupId = roleGroupId;
        RoleId = roleId;
    }
}
