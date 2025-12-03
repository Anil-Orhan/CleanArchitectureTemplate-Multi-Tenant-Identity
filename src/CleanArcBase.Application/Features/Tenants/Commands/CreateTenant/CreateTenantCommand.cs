using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Features.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string Identifier) : IRequest<Result<TenantDto>>;
