using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.UpdateRoleGroup;

public record UpdateRoleGroupCommand(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder) : IRequest<Result<RoleGroupDto>>;
