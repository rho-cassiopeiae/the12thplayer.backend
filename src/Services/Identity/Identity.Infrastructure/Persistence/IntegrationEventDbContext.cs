using Microsoft.EntityFrameworkCore;

using Identity.Application.Common.Integration;

namespace Identity.Infrastructure.Persistence {
    public class IntegrationEventDbContext : DbContext {
        public DbSet<IntegrationEvent> IntegrationEvents { get; set; }

        public IntegrationEventDbContext(
            DbContextOptions<IntegrationEventDbContext> options
        ) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.Entity<IntegrationEvent>(builder => {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Type).IsRequired();
                builder.Property(e => e.Payload).IsRequired();
                builder.Property(e => e.Status).IsRequired();
            });
        }
    }
}
