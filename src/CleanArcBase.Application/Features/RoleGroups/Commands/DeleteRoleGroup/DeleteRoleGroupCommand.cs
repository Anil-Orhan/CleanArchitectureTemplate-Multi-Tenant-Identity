using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.DeleteRoleGroup;

public record DeleteRoleGroupCommand(Guid Id) : IRequest<Result>;
