using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Application.Common.Attributes;
using MatchPredictions.Application.Common.Errors;
using MatchPredictions.Application.Common.Results;

namespace MatchPredictions.Application.Common.Interfaces {
    public interface IAuthorizationService {
        Task<Maybe<AuthorizationError>> Authorize(
            IAuthenticationContext authenticationContext,
            IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        );
    }
}
