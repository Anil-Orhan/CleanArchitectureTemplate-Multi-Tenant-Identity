using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(Guid Id, Guid? TenantId) : IRequest<Result>;
