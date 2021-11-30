using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Admin.Application.Common.Attributes;
using Admin.Application.Common.Interfaces;
using Admin.Application.Common.Results;

namespace Admin.Application.Common.Behaviors {
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<
        TRequest, TResponse
    > where TRequest : IRequest<TResponse>
      where TResponse : HandleResult, new() {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationBehavior(
            IAuthenticationContext authenticationContext,
            IAuthorizationService authorizationService
        ) {
            _authenticationContext = authenticationContext;
            _authorizationService = authorizationService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        ) {
            var authorizeAttributes = request
                .GetType()
                .GetCustomAttributes<RequireAuthorizationAttribute>();

            var outcome = await _authorizationService.AuthorizeAsync(
                _authenticationContext, authorizeAttributes
            );
            if (outcome.IsError) {
                return new TResponse {
                    Error = outcome.Error
                };
            }

            return await next();
        }
    }
}
