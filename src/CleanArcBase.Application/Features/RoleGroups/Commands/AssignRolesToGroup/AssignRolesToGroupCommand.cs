using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.AssignRolesToGroup;

public record AssignRolesToGroupCommand(
    Guid RoleGroupId,
    List<Guid> RoleIds) : IRequest<Result<RoleGroupDetailDto>>;
