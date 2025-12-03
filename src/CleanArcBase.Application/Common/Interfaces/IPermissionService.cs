namespace CleanArcBase.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
}
