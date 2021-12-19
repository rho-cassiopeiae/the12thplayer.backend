using System.Collections.Generic;
using System.Security.Claims;

using Identity.Application.Account.Common.Errors;
using Identity.Application.Common.Results;

namespace Identity.Application.Common.Interfaces {
    public interface ISecurityTokenProvider {
        string GenerateJwt(long sub, IList<Claim> claims = null);
        string GenerateRefreshToken();
        Either<AccountError, ClaimsPrincipal> CreatePrincipalFromAccessToken(string accessToken);
    }
}
