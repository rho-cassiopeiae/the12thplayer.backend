using System;
using System.Security.Claims;

namespace Profile.Application.Common.Interfaces {
    public interface IAuthenticationContext {
        ClaimsPrincipal User { get; set; }
        Exception Failure { get; set; }

        string GetFailureMessage();
    }
}
