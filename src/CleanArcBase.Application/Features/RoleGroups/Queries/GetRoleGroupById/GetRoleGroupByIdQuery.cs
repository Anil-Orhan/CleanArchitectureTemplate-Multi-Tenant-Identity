using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Queries.GetRoleGroupById;

public record GetRoleGroupByIdQuery(Guid Id) : IRequest<RoleGroupDetailDto?>;
