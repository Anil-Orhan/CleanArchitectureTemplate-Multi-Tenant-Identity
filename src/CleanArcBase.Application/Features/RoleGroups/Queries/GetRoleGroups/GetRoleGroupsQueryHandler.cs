using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;
using CleanArcBase.Application.Common.Extensions;

namespace CleanArcBase.Application.Features.RoleGroups.Queries.GetRoleGroups;

public class GetRoleGroupsQueryHandler : IRequestHandler<GetRoleGroupsQuery, PagedResponse<RoleGroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoleGroupsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<RoleGroupDto>> Handle(GetRoleGroupsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.RoleGroups.Query()
            .Where(rg => request.TenantId == null || rg.TenantId == null || rg.TenantId == request.TenantId)
            .OrderBy(rg => rg.DisplayOrder)
            .ThenBy(rg => rg.Name);

        return await query.ToPagedResponseAsync(
            request.PageNumber,
            request.PageSize,
            rg => new RoleGroupDto
            {
                Id = rg.Id,
                Name = rg.Name,
                Description = rg.Description,
                IsSystem = rg.IsSystem,
                DisplayOrder = rg.DisplayOrder,
                TenantId = rg.TenantId,
                SourceRoleGroupId = rg.SourceRoleGroupId
            },
            cancellationToken);
    }
}
