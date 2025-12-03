using FluentValidation;

namespace CleanArcBase.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tenant name is required")
            .MinimumLength(2).WithMessage("Tenant name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Tenant name cannot exceed 200 characters");

        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Tenant identifier is required")
            .MinimumLength(2).WithMessage("Tenant identifier must be at least 2 characters")
            .MaximumLength(50).WithMessage("Tenant identifier cannot exceed 50 characters")
            .Matches(@"^[a-z0-9\-]+$").WithMessage("Tenant identifier can only contain lowercase letters, numbers and hyphens");
    }
}
