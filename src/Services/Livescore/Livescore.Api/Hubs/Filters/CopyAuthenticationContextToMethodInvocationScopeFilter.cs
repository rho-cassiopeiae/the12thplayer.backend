using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

using Livescore.Application.Common.Interfaces;

namespace Livescore.Api.Hubs.Filters {
    public class CopyAuthenticationContextToMethodInvocationScopeFilter : IHubFilter {
        public async ValueTask<object> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next
        ) {
            var attr = Attribute.GetCustomAttribute(
                invocationContext.HubMethod,
                typeof(CopyAuthenticationContextToMethodInvocationScopeAttribute)
            );
            if (attr != null) {
                var authenticationContextRequest = invocationContext
                    .Context
                    .GetHttpContext()
                    .RequestServices
                    .GetRequiredService<IAuthenticationContext>();
                var authenticationContextInvocation = invocationContext
                    .ServiceProvider
                    .GetRequiredService<IAuthenticationContext>();

                authenticationContextInvocation.User = authenticationContextRequest.User;
                authenticationContextInvocation.Token = authenticationContextRequest.Token;
                authenticationContextInvocation.Failure = authenticationContextRequest.Failure;
            }

            return await next(invocationContext);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CopyAuthenticationContextToMethodInvocationScopeAttribute : Attribute { }
}
