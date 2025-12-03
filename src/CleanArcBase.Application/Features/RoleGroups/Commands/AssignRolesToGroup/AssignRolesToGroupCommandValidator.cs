using FluentValidation;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.AssignRolesToGroup;

public class AssignRolesToGroupCommandValidator : AbstractValidator<AssignRolesToGroupCommand>
{
    public AssignRolesToGroupCommandValidator()
    {
        RuleFor(x => x.RoleGroupId)
            .NotEmpty().WithMessage("Role group ID is required");

        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("Role IDs list is required");
    }
}
