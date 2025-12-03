using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;
using CleanArcBase.Application.Common.Extensions;

namespace CleanArcBase.Application.Features.Roles.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedResponse<RoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRolesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Roles.Query()
            .Where(r => request.TenantId == null || r.TenantId == null || r.TenantId == request.TenantId)
            .OrderBy(r => r.Name);

        return await query.ToPagedResponseAsync(
            request.PageNumber,
            request.PageSize,
            r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name!,
                Description = r.Description,
                IsSystemRole = r.IsSystemRole,
                TenantId = r.TenantId
            },
            cancellationToken);
    }
}
