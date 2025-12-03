using CleanArcBase.Application.Common.Interfaces;
using MediatR;
using CleanArcBase.Application.Common.Models;

namespace CleanArcBase.Application.Features.RoleGroups.Queries.GetRoleGroups;

public record GetRoleGroupsQuery(Guid? TenantId, int PageNumber = 1, int PageSize = 10)
    : IPagedQuery<RoleGroupDto>;
