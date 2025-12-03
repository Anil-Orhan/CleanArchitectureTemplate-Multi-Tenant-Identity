using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using CleanArcBase.Application.Features.Roles;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.AssignRolesToGroup;

public class AssignRolesToGroupCommandHandler : IRequestHandler<AssignRolesToGroupCommand, Result<RoleGroupDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolesToGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleGroupDetailDto>> Handle(AssignRolesToGroupCommand request, CancellationToken cancellationToken)
    {
        var roleGroup = await _unitOfWork.RoleGroups.GetByIdWithRolesAsync(request.RoleGroupId, cancellationToken);

        if (roleGroup == null)
            return Result.Failure<RoleGroupDetailDto>("Role group not found");

        // Set the new roles (clears existing and adds new ones)
        roleGroup.SetRoles(request.RoleIds);

        _unitOfWork.RoleGroups.Update(roleGroup);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload to get updated roles
        roleGroup = await _unitOfWork.RoleGroups.GetByIdWithRolesAsync(request.RoleGroupId, cancellationToken);

        return Result.Success(new RoleGroupDetailDto
        {
            Id = roleGroup!.Id,
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
        });
    }
}
