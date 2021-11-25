using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Account {
    public static class IdentityResultExtension {
        public static Dictionary<string, string[]> ToErrorDictionary(
            this IdentityResult result
        ) => result.Errors.ToDictionary(
            error => error.Code,
            error => new[] { error.Description }
        );
    }
}
