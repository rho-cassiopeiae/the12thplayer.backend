using System.Collections.Generic;
using System.Linq;

using FluentValidation.Results;

namespace Livescore.Application.Common.Errors {
    public class ValidationError : HandleError {
        internal ValidationError(IEnumerable<ValidationFailure> failures) : base(type: "Validation") {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public ValidationError(string message) : base(type: "Validation") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
