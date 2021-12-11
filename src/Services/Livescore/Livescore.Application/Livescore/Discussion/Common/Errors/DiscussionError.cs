using System.Collections.Generic;

using Livescore.Application.Common.Errors;

namespace Livescore.Application.Livescore.Discussion.Common.Errors {
    public class DiscussionError : HandleError {
        public DiscussionError(string message) : base(type: "Discussion") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
