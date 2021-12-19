using System.Security.Claims;

namespace Identity.Application.Common.Interfaces {
    public interface IPrincipalDataProvider {
        long GetId(ClaimsPrincipal principal);
    }
}
