using CleanArcBase.Application.Common.Interfaces;
using MediatR;

namespace CleanArcBase.Application.Features.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, IReadOnlyList<PermissionGroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPermissionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PermissionGroupDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _unitOfWork.Permissions.GetAllAsync(cancellationToken);

        var grouped = permissions
            .GroupBy(p => p.Group.ToString())
            .Select(g => new PermissionGroupDto
            {
                Group = g.Key,
                Permissions = g.Select(p => new PermissionItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Group = p.Group.ToString(),
                    Description = p.Description
                }).ToList()
            })
            .ToList();

        return grouped;
    }
}
