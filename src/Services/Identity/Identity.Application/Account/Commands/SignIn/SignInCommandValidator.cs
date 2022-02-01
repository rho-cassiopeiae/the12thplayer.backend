using FluentValidation;

namespace Identity.Application.Account.Commands.SignIn {
    public class SignInCommandValidator : AbstractValidator<SignInCommand> {
        public SignInCommandValidator() {
            RuleFor(c => c.DeviceId).NotEmpty();
            RuleFor(c => c.Email).NotNull().EmailAddress();
            RuleFor(c => c.Password).NotEmpty().MaximumLength(32);
        }
    }
}
