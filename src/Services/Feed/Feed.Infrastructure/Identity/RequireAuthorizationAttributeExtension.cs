using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;

using Feed.Application.Common.Attributes;

namespace Feed.Infrastructure.Identity {
    public static class RequireAuthorizationAttributeExtension {
        public static IEnumerable<IAuthorizeData> ToAuthorizeData(
            this IEnumerable<RequireAuthorizationAttribute> authorizeAttributes
        ) => authorizeAttributes.Select(
            a => new AuthorizeAttribute { Policy = a.Policy, Roles = a.Roles }
        );
    }
}
