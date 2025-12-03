using CleanArcBase.Application.Common.Interfaces;

namespace CleanArcBase.Application.Features.Roles.Queries.GetRoles;

public record GetRolesQuery(Guid? TenantId, int PageNumber = 1, int PageSize = 10)
    : IPagedQuery<RoleDto>;
