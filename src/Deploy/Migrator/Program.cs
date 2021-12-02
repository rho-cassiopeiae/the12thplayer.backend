using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Identity.Infrastructure.Persistence;

using Profile.Infrastructure.Persistence;

using Livescore.Infrastructure.Persistence;

namespace Migrator {
    class Program {
        static void Main(string[] args) {
            var host = Identity.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var userDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<UserDbContext>();

                userDbContext.Database.Migrate();
            }

            using (var scope = host.Services.CreateScope()) {
                var integrationEventDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<IntegrationEventDbContext>();

                integrationEventDbContext.Database.Migrate();
            }

            host = Profile.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var profileDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<ProfileDbContext>();

                profileDbContext.Database.Migrate();
            }

            host = Livescore.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var livescoreDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<LivescoreDbContext>();

                livescoreDbContext.Database.Migrate();
            }
        }
    }
}
