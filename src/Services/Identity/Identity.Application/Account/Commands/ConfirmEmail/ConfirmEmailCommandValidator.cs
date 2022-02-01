using FluentValidation;

namespace Identity.Application.Account.Commands.ConfirmEmail {
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand> {
        public ConfirmEmailCommandValidator() {
            RuleFor(c => c.Email).NotNull().EmailAddress();
            RuleFor(c => c.ConfirmationCode).Must(value =>
                value != null && value.Length == 6 && int.TryParse(value, out _)
            );
        }
    }
}
