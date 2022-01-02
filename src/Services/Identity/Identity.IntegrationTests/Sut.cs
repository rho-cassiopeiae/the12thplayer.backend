using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Respawn;
using MediatR;
using Xunit.Abstractions;

using Identity.Api;
using Identity.Infrastructure.Persistence;
using Identity.Application.Common.Interfaces;
using Identity.IntegrationTests.Account.Mocks;

namespace Identity.IntegrationTests {
    public class Sut {
        private readonly IHost _host;
        private readonly Checkpoint _checkpoint;

        public Sut(IMessageSink sink) {
            _host = Program
                .CreateHostBuilder(args: null)
                .ConfigureAppConfiguration((hostContext, builder) => {
                    builder.AddJsonFile("appsettings.Testing.json", optional: false);
                })
                .ConfigureLogging((hostContext, builder) => {
                    builder.Services.TryAddEnumerable(
                        ServiceDescriptor.Singleton<ILoggerProvider>(new XunitLoggerProvider(sink))
                    );
                })
                .ConfigureServices(services => {
                    services.AddScoped<IUserService, UserServiceMock>();
                })
                .Build();

            _checkpoint = new Checkpoint {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] {
                    "identity"
                },
                TablesToIgnore = new[] {
                    "__EFMigrationsHistory_UserDbContext",
                    "__EFMigrationsHistory_IntegrationEventDbContext"
                }
            };

            _applyMigrations();
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

        private void _applyMigrations() {
            using var scope = _host.Services.CreateScope();

            var userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            userDbContext.Database.Migrate();

            var integrationEventDbContext = scope.ServiceProvider.GetRequiredService<IntegrationEventDbContext>();
            integrationEventDbContext.Database.Migrate();
        }

        public void ResetState() {
            using var scope = _host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            context.Database.OpenConnection();
            _checkpoint.Reset(context.Database.GetDbConnection()).Wait();
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
