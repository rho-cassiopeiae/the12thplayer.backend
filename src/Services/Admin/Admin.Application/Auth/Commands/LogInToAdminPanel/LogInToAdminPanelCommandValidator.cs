using FluentValidation;

namespace Admin.Application.Auth.Commands.LogInToAdminPanel {
    public class LogInToAdminPanelCommandValidator : AbstractValidator<LogInToAdminPanelCommand> {
        public LogInToAdminPanelCommandValidator() {
            RuleFor(c => c.Email).NotNull().EmailAddress();
            RuleFor(c => c.Password).NotEmpty().MaximumLength(32);
        }
    }
}
