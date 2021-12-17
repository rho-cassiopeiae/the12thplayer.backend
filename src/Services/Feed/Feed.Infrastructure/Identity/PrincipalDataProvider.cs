using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Feed.Application.Common.Interfaces;

namespace Feed.Infrastructure.Identity {
    public class PrincipalDataProvider : IPrincipalDataProvider {
        public long GetId(ClaimsPrincipal principal) =>
            long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub));

        public string GetUsername(ClaimsPrincipal principal) => principal.FindFirstValue("__Username");
    }
}
