using System;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Responses.Identity;

using Admin.Application.Common.Errors;
using Admin.Application.Common.Results;
using Admin.Application.Common.Interfaces;

namespace Admin.Infrastructure.Auth {
    public class AuthService : IAuthService {
        private readonly IRequestClient<LogInAsAdmin> _logInClient;

        public AuthService(IRequestClient<LogInAsAdmin> logInClient) {
            _logInClient = logInClient;
        }

        public async Task<Either<AuthError, string>> LogInAsAdmin(
            string email, string password
        ) {
            var response = await _logInClient.GetResponse<LogInAsAdminSuccess, LogInAsAdminError>(
                new LogInAsAdmin {
                    CorrelationId = Guid.NewGuid(),
                    Email = email,
                    Password = password
                }
            );

            if (response.Is<LogInAsAdminError>(out var errorResult)) {
                return new AuthError(errorResult.Message.Message);
            }

            response.Is<LogInAsAdminSuccess>(out var result);

            return result.Message.AccessToken;
        }
    }
}
