using System.Collections.Generic;

namespace Admin.Application.Common.Errors {
    public class AuthError : HandleError {
        public AuthError(string message) : base(type: "Auth") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
