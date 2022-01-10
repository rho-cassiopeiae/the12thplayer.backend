using System.Collections.Generic;

namespace Feed.Application.Common.Errors {
    public class FeedError : HandleError {
        public FeedError(string message) : base(type: "Feed") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
