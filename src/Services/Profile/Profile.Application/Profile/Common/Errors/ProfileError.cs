using System.Collections.Generic;

using Profile.Application.Common.Errors;

namespace Profile.Application.Profile.Common.Errors {
    public class ProfileError : HandleError {
        public ProfileError(string message) : base(type: "Profile") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
