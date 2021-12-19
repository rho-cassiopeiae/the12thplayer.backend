using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Identity.Application.Common.Interfaces;

namespace Identity.Infrastructure.Account {
    public class PrincipalDataProvider : IPrincipalDataProvider {
        public long GetId(ClaimsPrincipal principal) =>
            long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
    }
}
