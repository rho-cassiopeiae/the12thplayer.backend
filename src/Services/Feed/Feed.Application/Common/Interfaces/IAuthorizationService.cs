using System.Collections.Generic;
using System.Threading.Tasks;

using Feed.Application.Common.Attributes;
using Feed.Application.Common.Errors;
using Feed.Application.Common.Results;

namespace Feed.Application.Common.Interfaces {
    public interface IAuthorizationService {
        Task<Maybe<AuthorizationError>> Authorize(
            IAuthenticationContext authenticationContext,
            IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        );
    }
}
