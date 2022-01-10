using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using Feed.Application.Common.Interfaces;

namespace Feed.Infrastructure.Identity {
    public class AuthenticationContext : IAuthenticationContext {
        public ClaimsPrincipal User { get; set; }
        public SecurityToken Token { get; set; }
        public Exception Failure { get; set; }

        public string GetFailureMessage() {
            if (Failure == null) {
                return "Unauthenticated";
            }

            IReadOnlyCollection<Exception> exceptions;
            if (Failure is AggregateException aggFailure) {
                exceptions = aggFailure.InnerExceptions;
            } else {
                exceptions = new[] { Failure };
            }

            var messages = new List<string>(exceptions.Count);

            foreach (var ex in exceptions) {
                switch (ex) {
                    case SecurityTokenInvalidAudienceException stia:
                        messages.Add($"The audience '{stia.InvalidAudience ?? "(null)"}' is invalid");
                        break;
                    case SecurityTokenInvalidIssuerException stii:
                        messages.Add($"The issuer '{stii.InvalidIssuer ?? "(null)"}' is invalid");
                        break;
                    case SecurityTokenNoExpirationException _:
                        messages.Add("The token has no expiration");
                        break;
                    case SecurityTokenInvalidLifetimeException stil:
                        messages.Add("The token lifetime is invalid; NotBefore: "
                            + $"'{stil.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'"
                            + $", Expires: '{stil.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'");
                        break;
                    case SecurityTokenNotYetValidException stnyv:
                        messages.Add($"The token is not valid before '{stnyv.NotBefore.ToString(CultureInfo.InvariantCulture)}'");
                        break;
                    case SecurityTokenExpiredException ste:
                        messages.Add($"The token expired at '{ste.Expires.ToString(CultureInfo.InvariantCulture)}'");
                        break;
                    case SecurityTokenSignatureKeyNotFoundException _:
                        messages.Add("The signature key was not found");
                        break;
                    case SecurityTokenInvalidSignatureException _:
                        messages.Add("The signature is invalid");
                        break;
                }
            }

            return string.Join("; ", messages);
        }
    }
}
