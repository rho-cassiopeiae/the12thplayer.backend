using System.Linq;

using FluentValidation;

namespace Identity.Application.Account.Commands.SignUp {
    public class SignUpCommandValidator : AbstractValidator<SignUpCommand> {
        const string _allowedUsernameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._";

        public SignUpCommandValidator() {
            RuleFor(c => c.Email).NotNull().EmailAddress();
            RuleFor(c => c.Username).Must(value => value.All(c => _allowedUsernameCharacters.Contains(c)));
            RuleFor(c => c.Password).NotEmpty().MaximumLength(32);
        }
    }
}
