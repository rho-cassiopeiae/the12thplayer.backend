using System.Collections.Generic;

namespace Admin.Application.Common.Errors {
    public class PermissionError : HandleError {
        public PermissionError(string message) : base(type: "Permission") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
