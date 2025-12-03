using MediatR;

namespace CleanArcBase.Application.Features.Roles.Queries.GetRoleById;

public record GetRoleByIdQuery(Guid Id, Guid? TenantId) : IRequest<RoleDetailDto?>;
