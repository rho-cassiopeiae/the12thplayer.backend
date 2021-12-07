using System.Collections.Generic;

using Livescore.Application.Common.Errors;

namespace Livescore.Application.Livescore.Common.Errors {
    public class LivescoreError : HandleError {
        public LivescoreError(string message) : base(type: "Livescore") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
