using System.Collections.Generic;

namespace Worker.Application.Common.Errors {
    public abstract class HandleError {
        public string Type { get; }
        public IReadOnlyDictionary<string, string[]> Errors { get; protected set; }

        protected HandleError(string type) {
            Type = type;
        }
    }
}
