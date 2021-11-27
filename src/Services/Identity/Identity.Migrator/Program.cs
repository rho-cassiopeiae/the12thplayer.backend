using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Identity.Infrastructure.Account.Persistence;

namespace Identity.Migrator {
    class Program {
        static void Main(string[] args) {
            var host = Api.Program.CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var userDbContext = scope
                .ServiceProvider
                .GetRequiredService<UserDbContext>();

            userDbContext.Database.Migrate();
        }
    }
}
