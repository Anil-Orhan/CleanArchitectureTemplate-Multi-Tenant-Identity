using CleanArcBase.Application.Common.Interfaces;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleDetailDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(request.Id, cancellationToken);

        if (role == null)
            return null;

        if (role.TenantId != null && role.TenantId != request.TenantId)
            return null;

        return new RoleDetailDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            TenantId = role.TenantId,
            Permissions = role.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                DisplayName = rp.Permission.DisplayName
            }).ToList()
        };
    }
}
