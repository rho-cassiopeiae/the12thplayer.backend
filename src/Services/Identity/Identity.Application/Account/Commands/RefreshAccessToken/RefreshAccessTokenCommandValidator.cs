using FluentValidation;

namespace Identity.Application.Account.Commands.RefreshAccessToken {
    public class RefreshAccessTokenCommandValidator : AbstractValidator<RefreshAccessTokenCommand> {
        public RefreshAccessTokenCommandValidator() {
            RuleFor(c => c.DeviceId).NotEmpty();
            RuleFor(c => c.AccessToken).NotEmpty();
            RuleFor(c => c.RefreshToken).NotEmpty();
        }
    }
}
