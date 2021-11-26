using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Application.Common.Interfaces {
    public interface ISecurityTokenProvider {
        string GenerateJwt(long sub, IList<Claim> claims = null);
        string GenerateRefreshToken();
    }
}
