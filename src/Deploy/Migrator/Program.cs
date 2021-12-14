using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using IdentitySvc  = Identity.Infrastructure.Persistence;
using ProfileSvc   = Profile.Infrastructure.Persistence;
using LivescoreSvc = Livescore.Infrastructure.Persistence;

namespace Migrator {
    class Program {
        public static void Main(string[] args) {
            var host = Identity.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var userDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<IdentitySvc.UserDbContext>();

                userDbContext.Database.Migrate();

                var integrationEventDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<IdentitySvc.IntegrationEventDbContext>();

                integrationEventDbContext.Database.Migrate();
            }

            host = Profile.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var profileDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<ProfileSvc.ProfileDbContext>();

                profileDbContext.Database.Migrate();

                var integrationEventDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<ProfileSvc.IntegrationEventDbContext>();

                integrationEventDbContext.Database.Migrate();
            }

            host = Livescore.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var livescoreDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<LivescoreSvc.LivescoreDbContext>();

                livescoreDbContext.Database.Migrate();
            }
        }
    }
}
