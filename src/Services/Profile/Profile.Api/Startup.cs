using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Profile.Api.Consumers.Identity;
using Profile.Api.HostedServices;
using Profile.Application;
using Profile.Infrastructure;

namespace Profile.Api {
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
                    busCfg.AddConsumer<UserAccountEventsConsumer>();
                    busCfg.AddConsumer<PermissionRequestsConsumer>();
                }
            );

            services.AddControllers();

            services.AddHostedService<IntegrationEventDispatcher>();
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
            });
        }
    }
}
