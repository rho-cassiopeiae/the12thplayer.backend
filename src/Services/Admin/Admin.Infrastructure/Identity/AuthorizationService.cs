using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Admin.Application.Common.Errors;
using Admin.Application.Common.Results;
using Admin.Application.Common.Attributes;
using Interfaces = Admin.Application.Common.Interfaces;

namespace Admin.Infrastructure.Identity {
    public class AuthorizationService : Interfaces.IAuthorizationService {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthorizationPolicyProvider _authorizationPolicyProvider;

        public AuthorizationService(
            IAuthorizationService authorizationService,
            IAuthorizationPolicyProvider authorizationPolicyProvider
        ) {
            _authorizationService = authorizationService;
            _authorizationPolicyProvider = authorizationPolicyProvider;
        }

        public async Task<Maybe<AuthorizationError>> AuthorizeAsync(
            Interfaces.IAuthenticationContext authenticationContext,
            IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        ) {
            var authorizeData = authorizeAttributes.ToAuthorizeData();
            var combinedPolicy = await AuthorizationPolicy.CombineAsync(
                _authorizationPolicyProvider, authorizeData
            );
            if (combinedPolicy == null) {
                return null;
            }

            if (authenticationContext.User == null) {
                return new AuthorizationError(
                    authenticationContext.GetFailureMessage()
                );
            }

            var result = await _authorizationService.AuthorizeAsync(
                authenticationContext.User, combinedPolicy
            );

            return !result.Succeeded ? new AuthorizationError("Forbidden") : null;
        }
    }
}
