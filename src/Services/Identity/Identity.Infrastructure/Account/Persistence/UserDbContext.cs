using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Identity.Infrastructure.Account.Persistence.Models;

namespace Identity.Infrastructure.Account.Persistence {
    public class UserDbContext : IdentityUserContext<User, long> {
        public DbSet<IntegrationEvent> IntegrationEvents { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.Entity<IntegrationEvent>(builder => {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Type).IsRequired();
                builder.Property(e => e.Content).IsRequired();
                builder.Property(e => e.Status).IsRequired();
            });
        }
    }
}
