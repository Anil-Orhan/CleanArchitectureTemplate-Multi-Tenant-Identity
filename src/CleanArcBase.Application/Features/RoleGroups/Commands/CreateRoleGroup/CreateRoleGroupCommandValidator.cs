using FluentValidation;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.CreateRoleGroup;

public class CreateRoleGroupCommandValidator : AbstractValidator<CreateRoleGroupCommand>
{
    public CreateRoleGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role group name is required")
            .MinimumLength(2).WithMessage("Role group name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Role group name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater");
    }
}
