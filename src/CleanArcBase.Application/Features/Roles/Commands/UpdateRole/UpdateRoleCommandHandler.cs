using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.Id, cancellationToken);

        if (role == null)
            return Result.Failure<RoleDto>("Role not found");

        if (role.IsSystemRole)
            return Result.Failure<RoleDto>("System roles cannot be modified");

        if (role.TenantId != null && role.TenantId != request.TenantId)
            return Result.Failure<RoleDto>("Role not found");

        var isUnique = await _unitOfWork.Roles.IsNameUniqueAsync(
            request.Name,
            request.TenantId,
            request.Id,
            cancellationToken);

        if (!isUnique)
            return Result.Failure<RoleDto>("A role with this name already exists");

        role.Update(request.Name, request.Description);
        _unitOfWork.Roles.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            TenantId = role.TenantId
        });
    }
}
