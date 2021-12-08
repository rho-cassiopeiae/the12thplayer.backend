using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Application.Common.Attributes;
using Livescore.Application.Common.Errors;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Common.Interfaces {
    public interface IAuthorizationService {
        Task<Maybe<AuthorizationError>> Authorize(
            IAuthenticationContext authenticationContext,
            IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        );
    }
}
