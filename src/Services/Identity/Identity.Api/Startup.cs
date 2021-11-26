using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Identity.Application;
using Identity.Infrastructure;
using Identity.Api.Controllers.Filters;
using Identity.Api.HostedServices;

namespace Identity.Api {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            services
                .AddControllers(options => {
                    options.Filters.Add<ConvertHandleResultToMvcResponseFilter>();
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase;
                });

            services.AddHostedService<IntegrationEventDispatcher>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
