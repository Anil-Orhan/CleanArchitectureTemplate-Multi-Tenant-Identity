using FluentValidation;

namespace CleanArcBase.Application.Features.RoleGroups.Commands.DeleteRoleGroup;

public class DeleteRoleGroupCommandValidator : AbstractValidator<DeleteRoleGroupCommand>
{
    public DeleteRoleGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role group ID is required");
    }
}
