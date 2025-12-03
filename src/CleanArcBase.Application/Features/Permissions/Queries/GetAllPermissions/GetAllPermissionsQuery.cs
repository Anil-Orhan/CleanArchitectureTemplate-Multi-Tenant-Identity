using MediatR;

namespace CleanArcBase.Application.Features.Permissions.Queries.GetAllPermissions;

public record GetAllPermissionsQuery : IRequest<IReadOnlyList<PermissionGroupDto>>;
