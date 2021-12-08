using System.Security.Claims;

namespace Livescore.Application.Common.Interfaces {
    public interface IPrincipalDataProvider {
        long GetId(ClaimsPrincipal principal);
        string GetUsername(ClaimsPrincipal principal);
    }
}
