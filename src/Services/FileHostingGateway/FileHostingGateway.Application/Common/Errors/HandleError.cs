using System.Collections.Generic;

namespace FileHostingGateway.Application.Common.Errors {
    public abstract class HandleError {
        public string Type { get; }
        public IReadOnlyDictionary<string, string[]> Errors { get; protected set; }

        protected HandleError(string type) {
            Type = type;
        }
    }
}
