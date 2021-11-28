using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Identity.Infrastructure.Persistence.Models;

namespace Identity.Infrastructure.Persistence {
    public class UserDbContext : IdentityUserContext<User, long> {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.Entity<RefreshToken>(builder => {
                builder.HasKey(t => new { t.UserId, t.Value });
                builder
                    .HasOne<User>()
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(t => t.UserId)
                    .IsRequired();
                builder.Property(t => t.IsActive).IsRequired();
                builder.Property(t => t.ExpiresAt).IsRequired();
            });
        }
    }
}
