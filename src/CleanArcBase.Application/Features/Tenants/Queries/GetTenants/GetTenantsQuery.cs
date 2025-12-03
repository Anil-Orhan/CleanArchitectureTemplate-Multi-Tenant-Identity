using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;

namespace CleanArcBase.Application.Features.Tenants.Queries.GetTenants;

public record GetTenantsQuery(int PageNumber = 1, int PageSize = 10)
    : IPagedQuery<TenantDto>;
