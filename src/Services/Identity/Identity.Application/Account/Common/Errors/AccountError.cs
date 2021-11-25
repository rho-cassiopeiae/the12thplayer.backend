using System.Collections.Generic;

using Identity.Application.Common.Errors;

namespace Identity.Application.Account.Common.Errors {
    public class AccountError : HandleError {
        public AccountError(Dictionary<string, string[]> errors) :
            base(type: "Account") {
            Errors = errors;
        }

        public AccountError(string message) : base(type: "Account") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
