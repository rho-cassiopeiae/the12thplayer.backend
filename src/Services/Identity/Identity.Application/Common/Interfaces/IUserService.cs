using System.Threading.Tasks;

using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;

namespace Identity.Application.Common.Interfaces {
    public interface IUserService {
        Task<Either<AccountError, string>> Create(User user, string password);
    }
}
