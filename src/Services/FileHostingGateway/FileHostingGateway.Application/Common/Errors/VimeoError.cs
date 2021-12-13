using System.Collections.Generic;

namespace FileHostingGateway.Application.Common.Errors {
    public class VimeoError : HandleError {
        public VimeoError(string message) : base(type: "Vimeo") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
