using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;
using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.CreateRoleGroup;

public class CreateRoleGroupCommandHandler : IRequestHandler<CreateRoleGroupCommand, Result<RoleGroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleGroupDto>> Handle(CreateRoleGroupCommand request, CancellationToken cancellationToken)
    {
        var isUnique = await _unitOfWork.RoleGroups.IsNameUniqueAsync(
            request.Name,
            request.TenantId,
            cancellationToken: cancellationToken);

        if (!isUnique)
            return Result.Failure<RoleGroupDto>("A role group with this name already exists");

        var roleGroup = new RoleGroup(
            request.Name,
            request.Description,
            isSystem: request.TenantId == null,
            request.DisplayOrder,
            request.TenantId);

        // Assign roles if provided
        if (request.RoleIds?.Any() == true)
        {
            foreach (var roleId in request.RoleIds)
            {
                roleGroup.AssignRole(roleId);
            }
        }

        await _unitOfWork.RoleGroups.AddAsync(roleGroup, cancellationToken);
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
