using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;
using CleanArcBase.Application.Common.Extensions;

namespace CleanArcBase.Application.Features.Tenants.Queries.GetTenants;

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, PagedResponse<TenantDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTenantsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<TenantDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Tenants.Query()
            .OrderBy(t => t.Name);

        return await query.ToPagedResponseAsync(
            request.PageNumber,
            request.PageSize,
            t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Identifier = t.Identifier,
                IsActive = t.IsActive
            },
            cancellationToken);
    }
}
