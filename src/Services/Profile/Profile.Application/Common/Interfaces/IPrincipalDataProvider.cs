using System.Security.Claims;

namespace Profile.Application.Common.Interfaces {
    public interface IPrincipalDataProvider {
        long GetId(ClaimsPrincipal principal);
        string GetUsername(ClaimsPrincipal principal);
    }
}
