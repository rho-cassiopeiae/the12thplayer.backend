using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using FluentValidation;

using Identity.Application.Common.Errors;
using Identity.Application.Common.Results;

namespace Identity.Application.Common.Behaviors {
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : HandleResult, new() {

        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next
        ) {
            if (_validators.Any()) {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(validator => validator.ValidateAsync(context, cancellationToken))
                );
                var failures = validationResults
                    .SelectMany(result => result.Errors)
                    .ToList();

                if (failures.Any()) {
                    return new TResponse {
                        Error = new ValidationError(failures)
                    };
                }
            }

            return await next();
        }
    }
}
