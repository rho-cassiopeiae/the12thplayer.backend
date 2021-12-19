using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using MediatR;

using Identity.Infrastructure.Account;
using Identity.Infrastructure.Persistence;
using UserPm = Identity.Infrastructure.Persistence.Models.User;
using UserDm = Identity.Domain.Aggregates.User.User;

namespace Identity.IntegrationTests.Account.Mocks {
    public class UserServiceMock : UserService {
        public UserServiceMock(
            UserDbContext userDbContext,
            UserManager<UserPm> userManager,
            SignInManager<UserPm> signInManager,
            IPublisher mediator
        ) : base(
            userDbContext,
            userManager,
            signInManager,
            mediator
        ) { }

        public override Task<bool> VerifyEmailConfirmationCode(UserDm userDm, string confirmationCode) {
            return Task.FromResult(true);
        }
    }
}
