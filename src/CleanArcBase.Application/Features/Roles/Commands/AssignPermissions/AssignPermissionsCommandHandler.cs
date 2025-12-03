using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.AssignPermissions;

public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignPermissionsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(request.RoleId, cancellationToken);

        if (role == null)
            return Result.Failure("Role not found");

        if (role.TenantId != null && role.TenantId != request.TenantId)
            return Result.Failure("Role not found");

        role.ClearPermissions();

        foreach (var permissionId in request.PermissionIds)
        {
            role.AssignPermission(permissionId);
        }

        _unitOfWork.Roles.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
