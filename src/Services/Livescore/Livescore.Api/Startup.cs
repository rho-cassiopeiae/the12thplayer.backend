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
using Livescore.Api.Services.FixtureDiscussionBroadcaster;

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
                    busCfg.AddConsumer<LivescoreEventsConsumer>();
                    busCfg.AddConsumer<QueryRequestsConsumer>();
                }
            );

            services.AddControllers();

            services.TryAddSingleton<IFixtureDiscussionBroadcaster, FixtureDiscussionBroadcaster>();

            services.AddHostedService<FixtureDiscussionUpdatesDispatcher>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
