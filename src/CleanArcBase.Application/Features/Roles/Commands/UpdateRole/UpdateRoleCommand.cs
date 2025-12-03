using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? TenantId) : IRequest<Result<RoleDto>>;
