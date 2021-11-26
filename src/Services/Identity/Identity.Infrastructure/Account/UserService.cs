using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        private readonly UserDbContext _userDbContext;
        private readonly UserManager<UserPm> _userManager;

        private readonly Dictionary<UserDm, UserPm> _userDmToPm = new();
        
        public UserService(
            UserDbContext userDbContext,
            UserManager<UserPm> userManager
        ) {
            _userDbContext = userDbContext;
            _userManager = userManager;
        }

        public async Task<Either<AccountError, UserDm>> FindByEmail(string email) {
            var userPm = await _userManager.FindByEmailAsync(email);
            if (userPm == null) {
                return new AccountError($"Account {email} does not exist");
            }

            var userDm = new UserDm(
                id: userPm.Id,
                email: userPm.Email,
                username: userPm.UserName,
                isConfirmed: userPm.EmailConfirmed
            );

            _userDmToPm[userDm] = userPm;

            return userDm;
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
                // @@NOTE: Since auto-saving is enabled by default, a possible
                // error is detected right away.
                return new AccountError(result.ToErrorDictionary());
            }

            var token =
                await _userManager.GenerateEmailConfirmationTokenAsync(userPm);

            var eventPayload = new Dictionary<string, string> {
                ["email"] = userPm.Email,
                ["username"] = userPm.UserName
            };

            using var @event = new IntegrationEvent {
                Type = IntegrationEventType.UserAccountCreated,
                Payload = JsonDocument.Parse(JsonSerializer.Serialize(eventPayload)),
                Status = IntegrationEventStatus.Pending
            };

            _userDbContext.IntegrationEvents.Add(@event);

            await _userDbContext.SaveChangesAsync();

            await txn.CommitAsync();

            return token;
        }

        public Task<bool> VerifyEmailConfirmationCode(
            UserDm userDm, string confirmationCode
        ) {
            var userPm = _userDmToPm[userDm];

            return _userManager.VerifyUserTokenAsync(
                userPm,
                "ConfirmationCodeProvider",
                UserManager<UserPm>.ConfirmEmailTokenPurpose,
                confirmationCode
            );
        }

        public async Task<Maybe<AccountError>> FinalizeAccountCreation(UserDm userDm) {
            var userPm = _userDmToPm[userDm];

            using var txn = await _userDbContext.Database.BeginTransactionAsync();

            userPm.EmailConfirmed = userDm.IsConfirmed;
            userPm.RefreshTokens.AddRange(userDm.RefreshTokens.Select(
                token => new RefreshToken {
                    UserId = userPm.Id,
                    Value = token.Value,
                    IsActive = token.IsActive,
                    ExpiresAt = token.ExpiresAt.ToUnixTimeMilliseconds()
                }
            ));

            _userDbContext.AddRange(userPm.RefreshTokens);

            // @@NOTE: Need to explicitly start tracking refresh tokens in Added state
            // because UpdateAsync under the hood does this:
            //
            // Context.Attach(user);
            // user.ConcurrencyStamp = Guid.NewGuid().ToString();
            // Context.Update(user);
            //
            // Attaching the user will attach his refresh tokens in Unchanged state,
            // since tokens have non-generated keys. The following update will see
            // that the tokens are already being tracked, so will do nothing.

            var result = await _userManager.UpdateAsync(userPm);
            if (!result.Succeeded) {
                return new AccountError(result.ToErrorDictionary());
            }

            result = await _userManager.AddClaimsAsync(userPm, userDm.Claims);
            if (!result.Succeeded) {
                return new AccountError(result.ToErrorDictionary());
            }

            var eventPayload = new Dictionary<string, string> {
                ["id"] = userPm.Id.ToString()
            };

            using var @event = new IntegrationEvent {
                Type = IntegrationEventType.UserAccountConfirmed,
                Payload = JsonDocument.Parse(JsonSerializer.Serialize(eventPayload)),
                Status = IntegrationEventStatus.Pending
            };

            _userDbContext.IntegrationEvents.Add(@event);

            await _userDbContext.SaveChangesAsync();

            await txn.CommitAsync();

            return null;
        }
    }
}
