using System.Collections.Generic;

namespace Worker.Application.Common.Errors {
    public class ValidationError : HandleError {
        public ValidationError(string message) : base(type: "Validation") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
