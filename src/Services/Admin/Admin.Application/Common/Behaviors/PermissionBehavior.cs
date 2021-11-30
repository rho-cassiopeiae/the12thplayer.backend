using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Admin.Application.Common.Interfaces;
using Admin.Application.Common.Results;
using Admin.Application.Common.Attributes;
using Admin.Application.Common.Errors;

namespace Admin.Application.Common.Behaviors {
    public class PermissionBehavior<TRequest, TResponse> : IPipelineBehavior<
        TRequest, TResponse
    > where TRequest : IRequest<TResponse>
      where TResponse : HandleResult, new() {

        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;
        private readonly IProfilePermissionChecker _profilePermissionChecker;

        public PermissionBehavior(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IProfilePermissionChecker profilePermissionChecker
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _profilePermissionChecker = profilePermissionChecker;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        ) {
            var permissionAttributes = request
                .GetType()
                .GetCustomAttributes<RequirePermissionAttribute>();

            if (permissionAttributes.Any()) {
                var userId = _principalDataProvider.GetId(
                    _authenticationContext.User
                );

                var hasRequiredPermissions = await _profilePermissionChecker
                    .HasPermissions(userId, permissionAttributes);

                if (!hasRequiredPermissions) {
                    return new TResponse {
                        Error = new PermissionError("Insufficient permissions")
                    };
                }
            }

            return await next();
        }
    }
}
