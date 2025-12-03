using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.AssignPermissions;

public record AssignPermissionsCommand(
    Guid RoleId,
    IEnumerable<Guid> PermissionIds,
    Guid? TenantId) : IRequest<Result>;
