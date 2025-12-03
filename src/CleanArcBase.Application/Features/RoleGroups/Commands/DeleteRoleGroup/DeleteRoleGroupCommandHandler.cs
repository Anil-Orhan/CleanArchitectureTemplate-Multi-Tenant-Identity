using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.DeleteRoleGroup;

public class DeleteRoleGroupCommandHandler : IRequestHandler<DeleteRoleGroupCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteRoleGroupCommand request, CancellationToken cancellationToken)
    {
        var roleGroup = await _unitOfWork.RoleGroups.GetByIdAsync(request.Id, cancellationToken);

        if (roleGroup == null)
            return Result.Failure("Role group not found");

        if (roleGroup.IsSystem && roleGroup.TenantId == null)
            return Result.Failure("Cannot delete system role groups");

        _unitOfWork.RoleGroups.Remove(roleGroup);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
