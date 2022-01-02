using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MediatR;
using MassTransit;

using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Results;
using Identity.Infrastructure.Persistence;
using Identity.Application.Account.Common.Errors;
using Identity.Application.Account.DomainEvents.UserAccountCreated;
using Identity.Application.Account.DomainEvents.UserAccountConfirmed;
using Identity.Domain.Base;
using UserDm = Identity.Domain.Aggregates.User.User;
using UserPm = Identity.Infrastructure.Persistence.Models.User;
using RefreshTokenDm = Identity.Domain.Aggregates.User.RefreshToken;
using RefreshTokenPm = Identity.Infrastructure.Persistence.Models.RefreshToken;

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

        public async Task<Either<AccountError, UserDm>> FindByEmail(
            string email, bool includeRefreshTokensAndClaims = false
        ) {
            UserPm userPm;
            if (includeRefreshTokensAndClaims) {
                userPm = await _userDbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Email == email);
            } else {
                userPm = await _userManager.FindByEmailAsync(email);
            }

            if (userPm == null) {
                return new AccountError($"Account {email} does not exist");
            }

            IEnumerable<Claim> claims = null;
            if (includeRefreshTokensAndClaims) {
                claims = await _userManager.GetClaimsAsync(userPm);
            }

            var userDm = new UserDm(
                id: userPm.Id,
                email: userPm.Email,
                username: userPm.UserName,
                isConfirmed: userPm.EmailConfirmed
            );

            if (includeRefreshTokensAndClaims) {
                foreach (var token in userPm.RefreshTokens) {
                    userDm.AddRefreshToken(new RefreshTokenDm(
                        deviceId: token.DeviceId,
                        value: token.Value,
                        isActive: token.IsActive,
                        expiresAt: DateTimeOffset.FromUnixTimeMilliseconds(token.ExpiresAt)
                    ));
                }
                foreach (var claim in claims) {
                    userDm.AddClaim(claim.Type, claim.Value);
                }
            }

            _userDmToPm[userDm] = userPm;

            return userDm;
        }

        public async Task<Either<AccountError, UserDm>> FindById(
            long id, bool includeRefreshTokensAndClaims = false
        ) {
            UserPm userPm;
            if (includeRefreshTokensAndClaims) {
                userPm = await _userDbContext.Users
                    .Include(u => u.RefreshTokens)
                    .SingleOrDefaultAsync(u => u.Id == id);
            } else {
                userPm = await _userManager.FindByIdAsync(id.ToString());
            }

            if (userPm == null) {
                return new AccountError($"Account does not exist");
            }

            IEnumerable<Claim> claims = null;
            if (includeRefreshTokensAndClaims) {
                claims = await _userManager.GetClaimsAsync(userPm);
            }

            var userDm = new UserDm(
                id: userPm.Id,
                email: userPm.Email,
                username: userPm.UserName,
                isConfirmed: userPm.EmailConfirmed
            );

            if (includeRefreshTokensAndClaims) {
                foreach (var token in userPm.RefreshTokens) {
                    userDm.AddRefreshToken(new RefreshTokenDm(
                        deviceId: token.DeviceId,
                        value: token.Value,
                        isActive: token.IsActive,
                        expiresAt: DateTimeOffset.FromUnixTimeMilliseconds(token.ExpiresAt)
                    ));
                }
                foreach (var claim in claims) {
                    userDm.AddClaim(claim.Type, claim.Value);
                }
            }

            _userDmToPm[userDm] = userPm;

            return userDm;
        }

        public async Task<Maybe<AccountError>> Create(UserDm userDm, string password) {
            var userPm = new UserPm {
                Email = userDm.Email,
                UserName = userDm.Username
            };

            var result = await _userManager.CreateAsync(userPm, password);
            if (!result.Succeeded) {
                // @@NOTE: Since auto-saving is enabled by default, a possible
                // error (like duplicate email/username) is detected right away.
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

        public virtual Task<bool> VerifyEmailConfirmationCode(UserDm userDm, string confirmationCode) {
            return _userManager.VerifyUserTokenAsync(
                _userDmToPm[userDm],
                "ConfirmationCodeProvider",
                UserManager<UserPm>.ConfirmEmailTokenPurpose,
                confirmationCode
            );
        }

        public async Task<Maybe<AccountError>> FinalizeAccountCreation(UserDm userDm) {
            var userPm = _userDmToPm[userDm];

            userPm.EmailConfirmed = userDm.IsConfirmed;

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

        public async Task<Maybe<AccountError>> VerifyPassword(UserDm userDm, string password) {
            var result = await _signInManager.CheckPasswordSignInAsync(
                _userDmToPm[userDm], password, lockoutOnFailure: true
            );

            return !result.Succeeded ? new AccountError(result.ToString()) : null;
        }

        public async Task FinalizeSignIn(UserDm userDm) {
            var userPm = _userDmToPm[userDm];
            var newRefreshToken = userDm.RefreshTokens.Last();

            _userDbContext.RemoveRange(
                userPm.RefreshTokens.Where(t => t.DeviceId == newRefreshToken.DeviceId)
            );
            _userDbContext.Add(new RefreshTokenPm {
                UserId = userPm.Id,
                DeviceId = newRefreshToken.DeviceId,
                Value = newRefreshToken.Value,
                IsActive = newRefreshToken.IsActive,
                ExpiresAt = newRefreshToken.ExpiresAt.ToUnixTimeMilliseconds()
            });

            await _userDbContext.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokens(UserDm userDm) {
            var userPm = _userDmToPm[userDm];

            _userDbContext.RemoveRange(userPm.RefreshTokens.Where(
                t => !userDm.RefreshTokens.Any(
                    token => token.DeviceId == t.DeviceId && token.Value == t.Value
                )
            ));
            _userDbContext.AddRange(
                userDm.RefreshTokens
                    .Where(t => !userPm.RefreshTokens.Any(
                        token => token.DeviceId == t.DeviceId && token.Value == t.Value
                    ))
                    .Select(t => new RefreshTokenPm {
                        UserId = userPm.Id,
                        DeviceId = t.DeviceId,
                        Value = t.Value,
                        IsActive = t.IsActive,
                        ExpiresAt = t.ExpiresAt.ToUnixTimeMilliseconds()
                    })
            );

            await _userDbContext.SaveChangesAsync();
        }
    }
}
