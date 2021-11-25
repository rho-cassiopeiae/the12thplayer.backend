using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Infrastructure.Account.Persistence;
using Identity.Application.Account.Common.Errors;
using Identity.Infrastructure.Account.Persistence.Models;
using UserDm = Identity.Domain.Aggregates.User.User;
using UserPm = Identity.Infrastructure.Account.Persistence.Models.User;

namespace Identity.Infrastructure.Account {
    public class UserService : IUserService {
        private UserDbContext _userDbContext;
        private UserManager<UserPm> _userManager;
        
        public UserService(
            UserDbContext userDbContext,
            UserManager<UserPm> userManager
        ) {
            _userDbContext = userDbContext;
            _userManager = userManager;
        }

        public async Task<Either<AccountError, string>> Create(
            UserDm userDm, string password
        ) {
            using var txn = await _userDbContext.Database.BeginTransactionAsync();

            var userPm = new UserPm {
                Email = userDm.Email,
                UserName = userDm.Username
            };

            var result = await _userManager.CreateAsync(userPm, password);
            if (!result.Succeeded) {
                return new AccountError(result.ToErrorDictionary());
            }

            var token =
                await _userManager.GenerateEmailConfirmationTokenAsync(userPm);

            var @event = new IntegrationEvent {
                Type = IntegrationEventType.UserAccountCreated,
                Content = userDm.Email,
                Status = IntegrationEventStatus.Pending
            };

            _userDbContext.IntegrationEvents.Add(@event);

            await _userDbContext.SaveChangesAsync();

            await txn.CommitAsync();

            return token;
        }
    }
}
