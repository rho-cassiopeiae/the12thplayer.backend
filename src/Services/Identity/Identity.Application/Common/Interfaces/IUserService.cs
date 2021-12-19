using System.Threading;
using System.Threading.Tasks;

using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;
using Identity.Domain.Base;

namespace Identity.Application.Common.Interfaces {
    public interface IUserService {
        void EnlistConnectionFrom(IUnitOfWork unitOfWork);
        void EnlistTransactionFrom(IUnitOfWork unitOfWork);
        void EnlistAsPartOf(IUnitOfWork unitOfWork);

        Task DispatchDomainEvents(CancellationToken cancellationToken);
        Task<Either<AccountError, User>> FindByEmail(string email, bool includeRefreshTokensAndClaims = false);
        Task<Either<AccountError, User>> FindById(long id, bool includeRefreshTokensAndClaims = false);
        Task<Maybe<AccountError>> Create(User user, string password);
        Task<bool> VerifyEmailConfirmationCode(User user, string confirmationCode);
        Task<Maybe<AccountError>> FinalizeAccountCreation(User user);
        Task<Maybe<AccountError>> VerifyPassword(User user, string password);
        Task FinalizeSignIn(User user);
        Task UpdateRefreshTokens(User user);
    }
}
