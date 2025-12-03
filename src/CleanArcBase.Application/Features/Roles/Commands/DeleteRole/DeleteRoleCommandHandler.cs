using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.Id, cancellationToken);

        if (role == null)
            return Result.Failure("Role not found");

        if (role.IsSystemRole)
            return Result.Failure("System roles cannot be deleted");

        if (role.TenantId != null && role.TenantId != request.TenantId)
            return Result.Failure("Role not found");

        _unitOfWork.Roles.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
