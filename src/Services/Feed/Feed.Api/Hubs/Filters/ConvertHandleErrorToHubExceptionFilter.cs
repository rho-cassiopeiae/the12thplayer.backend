using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Feed.Application.Common.Results;

namespace Feed.Api.Hubs.Filters {
    public class ConvertHandleErrorToHubExceptionFilter : IHubFilter {
        public async ValueTask<object> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next
        ) {
            var handleResult = (HandleResult) await next(invocationContext);
            var error = handleResult.Error;
            if (error != null) {
                throw new HubException($"[{error.Type}Error] {error.Errors.Values.First().First()}");
            }

            return handleResult;
        }
    }
}
