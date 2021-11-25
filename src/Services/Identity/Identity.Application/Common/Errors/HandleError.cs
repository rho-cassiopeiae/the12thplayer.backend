using System.Collections.Generic;

namespace Identity.Application.Common.Errors {
    public abstract class HandleError {
        public string Type { get; }
        public IDictionary<string, string[]> Errors { get; protected set; }

        public HandleError(string type) {
            Type = type;
        }
    }
}
