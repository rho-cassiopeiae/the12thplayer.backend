using System;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace Livescore.Application.Common.Interfaces {
    public interface IAuthenticationContext {
        ClaimsPrincipal User { get; set; }
        SecurityToken Token { get; set; }
        Exception Failure { get; set; }

        string GetFailureMessage();
    }
}
