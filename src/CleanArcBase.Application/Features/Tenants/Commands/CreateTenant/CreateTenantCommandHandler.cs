using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using MediatR;
using CleanArcBase.Domain.Entities.Tenancy;

namespace CleanArcBase.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvisioningService _tenantProvisioningService;

    public CreateTenantCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantProvisioningService tenantProvisioningService)
    {
        _unitOfWork = unitOfWork;
        _tenantProvisioningService = tenantProvisioningService;
    }

    public async Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Check if identifier is unique
        var isUnique = await _unitOfWork.Tenants.IsIdentifierUniqueAsync(
            request.Identifier.ToLowerInvariant(),
            cancellationToken: cancellationToken);

        if (!isUnique)
            return Result.Failure<TenantDto>("A tenant with this identifier already exists");

        // Create tenant
        var tenant = new Tenant(request.Name, request.Identifier);

        await _unitOfWork.Tenants.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Provision tenant with default role groups
        await _tenantProvisioningService.ProvisionTenantAsync(tenant.Id, cancellationToken);

        return Result.Success(new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Identifier = tenant.Identifier,
            IsActive = tenant.IsActive
        });
    }
}
