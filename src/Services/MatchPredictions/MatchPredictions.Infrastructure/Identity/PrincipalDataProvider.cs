using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using MatchPredictions.Application.Common.Interfaces;

namespace MatchPredictions.Infrastructure.Identity {
    public class PrincipalDataProvider : IPrincipalDataProvider {
        public long GetId(ClaimsPrincipal principal) =>
            long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        
        public string GetUsername(ClaimsPrincipal principal) => principal.FindFirstValue("__Username");
    }
}
