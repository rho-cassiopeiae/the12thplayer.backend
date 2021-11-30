using System.Security.Claims;

namespace Admin.Application.Common.Interfaces {
    public interface IPrincipalDataProvider {
        long GetId(ClaimsPrincipal principal);
    }
}
