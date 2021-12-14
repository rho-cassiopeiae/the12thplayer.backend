using System.Collections.Generic;
using System.Threading.Tasks;

using Profile.Application.Common.Attributes;
using Profile.Application.Common.Errors;
using Profile.Application.Common.Results;

namespace Profile.Application.Common.Interfaces {
    public interface IAuthorizationService {
        Task<Maybe<AuthorizationError>> Authorize(
            IAuthenticationContext authenticationContext,
            IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        );
    }
}
