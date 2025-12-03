using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand(
    string Name,
    string? Description,
    Guid? TenantId) : IRequest<Result<RoleDto>>;
