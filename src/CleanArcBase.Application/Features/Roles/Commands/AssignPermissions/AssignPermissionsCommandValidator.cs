using FluentValidation;

namespace CleanArcBase.Application.Features.Roles.Commands.AssignPermissions;

public class AssignPermissionsCommandValidator : AbstractValidator<AssignPermissionsCommand>
{
    public AssignPermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required");

        RuleFor(x => x.PermissionIds)
            .NotNull().WithMessage("Permission IDs collection is required");
    }
}
