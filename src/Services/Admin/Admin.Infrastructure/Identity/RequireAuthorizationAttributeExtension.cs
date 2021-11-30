using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;

using Admin.Application.Common.Attributes;

namespace Admin.Infrastructure.Identity {
    public static class RequireAuthorizationAttributeExtension {
        public static IEnumerable<IAuthorizeData> ToAuthorizeData(
            this IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        ) => authorizeAttributes.Select(
            a => new AuthorizeAttribute { Policy = a.Policy, Roles = a.Roles }
        );
    }
}
