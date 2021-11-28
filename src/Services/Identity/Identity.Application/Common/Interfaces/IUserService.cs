using System;
using System.Threading;
using System.Threading.Tasks;

using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;
using Identity.Domain.Base;

namespace Identity.Application.Common.Interfaces {
    public interface IUserService {
        void EnlistConnectionFrom(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        void EnlistTransactionFrom(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        void EnlistAsPartOf(IUnitOfWork unitOfWork) => throw new NotSupportedException();

        Task DispatchDomainEvents(User user, CancellationToken cancellationToken);
        Task<Either<AccountError, User>> FindByEmail(string email);
        Task<Maybe<AccountError>> Create(User user, string password);
        Task<bool> VerifyEmailConfirmationCode(User user, string confirmationCode);
        Task<Maybe<AccountError>> FinalizeAccountCreation(User user);
    }
}
