using System.Threading.Tasks;

using Admin.Application.Common.Errors;
using Admin.Application.Common.Results;

namespace Admin.Application.Common.Interfaces {
    public interface IAuthService {
        Task<Either<AuthError, string>> LogInAsAdmin(
            string email, string password
        );
    }
}
