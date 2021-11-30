using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Admin.Application.Common.Interfaces;

namespace Admin.Infrastructure.Identity {
    public class PrincipalDataProvider : IPrincipalDataProvider {
        public long GetId(ClaimsPrincipal principal) =>
            long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
    }
}
