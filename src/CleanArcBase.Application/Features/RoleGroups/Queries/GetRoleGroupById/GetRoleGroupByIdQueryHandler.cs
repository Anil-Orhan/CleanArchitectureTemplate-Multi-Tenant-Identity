using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Features.Roles;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Queries.GetRoleGroupById;

public class GetRoleGroupByIdQueryHandler : IRequestHandler<GetRoleGroupByIdQuery, RoleGroupDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoleGroupByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleGroupDetailDto?> Handle(GetRoleGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var roleGroup = await _unitOfWork.RoleGroups.GetByIdWithRolesAsync(request.Id, cancellationToken);

        if (roleGroup == null)
            return null;

        return new RoleGroupDetailDto
        {
            Id = roleGroup.Id,
            Name = roleGroup.Name,
            Description = roleGroup.Description,
            IsSystem = roleGroup.IsSystem,
            DisplayOrder = roleGroup.DisplayOrder,
            TenantId = roleGroup.TenantId,
            SourceRoleGroupId = roleGroup.SourceRoleGroupId,
            Roles = roleGroup.RoleGroupRoles.Select(rgr => new RoleDto
            {
                Id = rgr.Role.Id,
                Name = rgr.Role.Name!,
                Description = rgr.Role.Description,
                IsSystemRole = rgr.Role.IsSystemRole,
                TenantId = rgr.Role.TenantId
            }).ToList()
        };
    }
}
