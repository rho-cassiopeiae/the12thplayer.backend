using System;
using System.Security.Claims;

namespace Feed.Application.Common.Interfaces {
    public interface IAuthenticationContext {
        ClaimsPrincipal User { get; set; }
        Exception Failure { get; set; }

        string GetFailureMessage();
    }
}
