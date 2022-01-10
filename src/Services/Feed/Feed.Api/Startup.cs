using System;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Feed.Api.Consumers.Identity;
using Feed.Api.Consumers.Profile;
using Feed.Application;
using Feed.Infrastructure;
using Feed.Api.Hubs;
using Feed.Api.Hubs.Filters;
using Feed.Api.Controllers.Filters;

namespace Feed.Api {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddApplication();
            services.AddInfrastructure(Configuration,
                busCfg => {
                    busCfg.AddConsumer<UserAccountEventsConsumer>();
                    busCfg.AddConsumer<PermissionEventsConsumer>();
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
                .AddHubOptions<FeedHub>(options => {
                    options.KeepAliveInterval = TimeSpan.FromSeconds(90);
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(180);

                    options.AddFilter<ConvertHandleErrorToHubExceptionFilter>();
                    options.AddFilter<CopyAuthenticationContextToMethodInvocationScopeFilter>();
                })
                .AddJsonProtocol(options => {
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
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
                endpoints.MapHub<FeedHub>("/feed/hub");
            });
        }
    }
}
