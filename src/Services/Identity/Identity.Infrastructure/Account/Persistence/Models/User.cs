using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Account.Persistence.Models {
    public class User : IdentityUser<long> {
        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
