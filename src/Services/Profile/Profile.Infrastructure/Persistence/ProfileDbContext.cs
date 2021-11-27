using Microsoft.EntityFrameworkCore;

using Profile.Domain.Aggregates.Profile;
using ProfileDm = Profile.Domain.Aggregates.Profile.Profile;

namespace Profile.Infrastructure.Persistence {
    public class ProfileDbContext : DbContext {
        public DbSet<ProfileDm> Profiles { get; set; }

        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) :
            base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("profile");

            modelBuilder.Entity<ProfileDm>(builder => {
                builder.HasKey(p => p.UserId);
                builder.Property(p => p.UserId).ValueGeneratedNever();
                builder.Property(p => p.Email).IsRequired();
                builder.Property(p => p.Username).IsRequired();
                builder.Property(p => p.Reputation).IsRequired();

                builder.Metadata
                    .FindNavigation(nameof(ProfileDm.Permissions))
                    .SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<ProfilePermission>(builder => {
                builder.HasKey(pp => new { pp.UserId, pp.Scope });
                builder.Property(pp => pp.Flags).IsRequired();

                builder
                    .HasOne<ProfileDm>()
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(pp => pp.UserId)
                    .IsRequired();
            });
        }
    }
}
