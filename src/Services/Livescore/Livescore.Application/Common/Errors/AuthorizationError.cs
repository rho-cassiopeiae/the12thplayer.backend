using System.Collections.Generic;

namespace Livescore.Application.Common.Errors {
    public class AuthorizationError : HandleError {
        public AuthorizationError(string message) : base(type: "Authorization") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
