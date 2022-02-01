using FluentValidation;

namespace Admin.Application.Profile.Commands.GrantSuperuserPermissions {
    public class GrantSuperuserPermissionsCommandValidator : AbstractValidator<GrantSuperuserPermissionsCommand> {
        public GrantSuperuserPermissionsCommandValidator() {
            RuleFor(c => c.Payload).NotEmpty();
            RuleFor(c => c.Signature).NotEmpty();
        }
    }
}
