using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MediatR;
using Xunit.Abstractions;

using FileHostingGateway.Host;

namespace FileHostingGateway.IntegrationTests {
    public class Sut {
        private readonly IHost _host;

        public Sut(IMessageSink sink) {
            _host = Program
                .ConfigureServices(
                    Program
                        .CreateHostBuilder(args: null)
                        .ConfigureAppConfiguration((hostContext, builder) => {
                            builder.AddJsonFile("appsettings.Testing.json", optional: false);
                            builder.AddUserSecrets<Sut>(optional: false);
                        })
                        .ConfigureLogging((hostContext, builder) => {
                            builder.Services.TryAddEnumerable(
                                ServiceDescriptor.Singleton<ILoggerProvider>(new XunitLoggerProvider(sink))
                            );
                        })
                )
                .Build();
        }

        public async Task<TResult> ExecWithService<TService, TResult>(
            Func<TService, Task<TResult>> func
        ) {
            using var scope = _host.Services.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<TService>();
            var result = await func(service);

            return result;
        }

        public void StartHostedService<T>() where T : IHostedService {
            _host.Services
                .GetServices<IHostedService>()
                .OfType<T>()
                .First()
                .StartAsync(CancellationToken.None)
                .Wait();
        }

        public void StopHostedService<T>() where T : IHostedService {
            _host.Services
                .GetServices<IHostedService>()
                .OfType<T>()
                .First()
                .StopAsync(CancellationToken.None)
                .Wait();
        }

        public T GetConfigurationValue<T>(string key) {
            var configuration = _host.Services.GetRequiredService<IConfiguration>();
            return configuration.GetValue<T>(key);
        }

        public async Task<T> SendRequest<T>(IRequest<T> request) {
            using var scope = _host.Services.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await mediator.Send(request);

            return result;
        }
    }
}
