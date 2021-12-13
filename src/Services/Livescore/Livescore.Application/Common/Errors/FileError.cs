using System.Collections.Generic;

namespace Livescore.Application.Common.Errors {
    public class FileError : HandleError {
        public FileError(string message) : base(type: "File") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
