using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.UpdateRoleGroup;

public class UpdateRoleGroupCommandHandler : IRequestHandler<UpdateRoleGroupCommand, Result<RoleGroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleGroupDto>> Handle(UpdateRoleGroupCommand request, CancellationToken cancellationToken)
    {
        var roleGroup = await _unitOfWork.RoleGroups.GetByIdAsync(request.Id, cancellationToken);

        if (roleGroup == null)
            return Result.Failure<RoleGroupDto>("Role group not found");

        var isUnique = await _unitOfWork.RoleGroups.IsNameUniqueAsync(
            request.Name,
            roleGroup.TenantId,
            request.Id,
            cancellationToken);

        if (!isUnique)
            return Result.Failure<RoleGroupDto>("A role group with this name already exists");

        roleGroup.Update(request.Name, request.Description, request.DisplayOrder);

        _unitOfWork.RoleGroups.Update(roleGroup);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoleGroupDto
        {
            Id = roleGroup.Id,
            Name = roleGroup.Name,
            Description = roleGroup.Description,
            IsSystem = roleGroup.IsSystem,
            DisplayOrder = roleGroup.DisplayOrder,
            TenantId = roleGroup.TenantId,
            SourceRoleGroupId = roleGroup.SourceRoleGroupId
        });
    }
}
