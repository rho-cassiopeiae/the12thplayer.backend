using System.Collections.Generic;
using System.Threading.Tasks;

using Admin.Application.Common.Attributes;
using Admin.Application.Common.Errors;
using Admin.Application.Common.Results;

namespace Admin.Application.Common.Interfaces {
    public interface IAuthorizationService {
        Task<Maybe<AuthorizationError>> AuthorizeAsync(
            IAuthenticationContext authenticationContext,
            IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        );
    }
}
