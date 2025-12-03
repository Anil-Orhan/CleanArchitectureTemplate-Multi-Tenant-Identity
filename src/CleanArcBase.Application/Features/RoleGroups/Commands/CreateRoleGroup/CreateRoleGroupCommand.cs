using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.CreateRoleGroup;

public record CreateRoleGroupCommand(
    string Name,
    string? Description,
    int DisplayOrder,
    Guid? TenantId,
    List<Guid>? RoleIds) : IRequest<Result<RoleGroupDto>>;
