using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MediatR;
using MassTransit;

using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Infrastructure.Persistence;
using Identity.Application.Account.Common.Errors;
using Identity.Infrastructure.Persistence.Models;
using Identity.Application.Account.DomainEvents.UserAccountCreated;
using Identity.Application.Account.DomainEvents.UserAccountConfirmed;
using Identity.Domain.Base;
using UserDm = Identity.Domain.Aggregates.User.User;
using UserPm = Identity.Infrastructure.Persistence.Models.User;

namespace Identity.Infrastructure.Account {
    public class UserService : IUserService {
        private readonly UserDbContext _userDbContext;
        private readonly UserManager<UserPm> _userManager;
        private readonly SignInManager<UserPm> _signInManager;
        private readonly IPublisher _mediator;

        private IUnitOfWork _unitOfWork;

        private readonly Dictionary<UserDm, UserPm> _userDmToPm = new();
        
        public UserService(
            UserDbContext userDbContext,
            UserManager<UserPm> userManager,
            SignInManager<UserPm> signInManager,
            IPublisher mediator
        ) {
            _userDbContext = userDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }

        public void EnlistConnectionFrom(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _userDbContext.Database.SetDbConnection(unitOfWork.Connection);
        }

        public void EnlistTransactionFrom(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _userDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _userDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _userDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task DispatchDomainEvents(CancellationToken cancellationToken) {
            foreach (var userDm in _userDmToPm.Keys) {
                if (userDm.DomainEvents != null) {
                    foreach (var @event in userDm.DomainEvents) {
                        await _mediator.Publish(@event);
                    }
                }
            }
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

        public async Task<Maybe<AccountError>> Create(
            UserDm userDm, string password
        ) {
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

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userPm);

            userDm.AddDomainEvent(new UserAccountCreatedDomainEvent {
                Email = userPm.Email,
                Username = userPm.UserName,
                ConfirmationCode = code
            });

            _userDmToPm[userDm] = userPm;

            return null;
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

            userDm.AddDomainEvent(new UserAccountConfirmedDomainEvent {
                UserId = userPm.Id,
                Email = userPm.Email,
                Username = userPm.UserName
            });

            return null;
        }

        public async Task<Maybe<AccountError>> VerifyPassword(
            UserDm userDm, string password
        ) {
            var userPm = _userDmToPm[userDm];

            var result = await _signInManager.CheckPasswordSignInAsync(
                userPm, password, lockoutOnFailure: true
            );

            return !result.Succeeded ? new AccountError(result.ToString()) : null;
        }
    }
}
