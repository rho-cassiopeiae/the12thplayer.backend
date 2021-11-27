using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Identity.Infrastructure.Account.Persistence;

using Profile.Infrastructure.Persistence;

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

            host = Profile.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var profileDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<ProfileDbContext>();

                profileDbContext.Database.Migrate();
            }
        }
    }
}
