using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;
using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var isUnique = await _unitOfWork.Roles.IsNameUniqueAsync(
            request.Name,
            request.TenantId,
            cancellationToken: cancellationToken);

        if (!isUnique)
            return Result.Failure<RoleDto>("A role with this name already exists");

        var role = new ApplicationRole(
            request.Name,
            request.Description,
            isSystemRole: false,
            tenantId: request.TenantId);

        await _unitOfWork.Roles.AddAsync(role, cancellationToken);
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
