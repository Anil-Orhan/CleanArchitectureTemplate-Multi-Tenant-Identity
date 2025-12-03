using CleanArcBase.Application.Common.Interfaces;
using MediatR;

namespace CleanArcBase.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTenantByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(request.Id, cancellationToken);

        if (tenant == null)
            return null;

        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Identifier = tenant.Identifier,
            IsActive = tenant.IsActive
        };
    }
}
