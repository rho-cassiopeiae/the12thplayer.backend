using System;
using System.Text.Json;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Livescore.Api.Consumers.Worker;
using Livescore.Api.HostedServices;
using Livescore.Application;
using Livescore.Infrastructure;
using Livescore.Api.Services;
using Livescore.Api.Controllers.Filters;
using Livescore.Api.Hubs;
using Livescore.Api.Hubs.Filters;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Api {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddApplication();
            services.AddInfrastructure(
                Configuration,
                busCfg => {
                    busCfg.AddConsumer<SeedRequestsConsumer>();
                    busCfg.AddConsumer<FixtureEventsConsumer>();
                    busCfg.AddConsumer<QueryRequestsConsumer>();
                }
            );

            services
                .AddControllers(options => {
                    options.Filters.Add<ConvertHandleErrorToMvcResponseFilter>();
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services
                .AddSignalR()
                .AddHubOptions<FanzoneHub>(options => {
                    options.KeepAliveInterval = TimeSpan.FromSeconds(90);
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(180);

                    options.AddFilter<ConvertHandleErrorToHubExceptionFilter>();
                    options.AddFilter<CopyAuthenticationContextToMethodInvocationScopeFilter>();
                    options.AddFilter<AddConnectionIdProviderToMethodInvocationScopeFilter>();
                })
                .AddJsonProtocol(options => {
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddScoped<IConnectionIdProvider, ConnectionIdProvider>();

            services.TryAddSingleton<IFixtureLivescoreBroadcaster, FixtureLivescoreBroadcaster>();
            services.TryAddSingleton<IFixtureDiscussionBroadcaster, FixtureDiscussionBroadcaster>();

            services.AddHostedService<FixtureDiscussionDispatcher>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();

                // @@NOTE: In dev, user-uploaded images get saved to the wwwroot and served by the service.
                // But in prod, the intent is to upload images to an S3 bucket and serve from there.
                app.UseStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<FanzoneHub>("/livescore/fanzone");
            });
        }
    }
}
