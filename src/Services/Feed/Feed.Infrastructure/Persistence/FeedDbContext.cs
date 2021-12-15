using Microsoft.EntityFrameworkCore;

using Feed.Domain.Aggregates.Author;

namespace Feed.Infrastructure.Persistence {
    public class FeedDbContext : DbContext {
        public DbSet<Author> Authors { get; set; }

        public FeedDbContext(DbContextOptions<FeedDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("feed");

            modelBuilder.Entity<Author>(builder => {
                builder.HasKey(a => a.UserId);
                builder.Property(a => a.UserId).ValueGeneratedNever();
                builder.Property(a => a.Email).IsRequired();
                builder.Property(a => a.Username).IsRequired();

                builder.Metadata
                    .FindNavigation(nameof(Author.Permissions))
                    .SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<AuthorPermission>(builder => {
                builder.HasKey(ap => new { ap.UserId, ap.Scope });
                builder.Property(ap => ap.Flags).IsRequired();

                builder
                    .HasOne<Author>()
                    .WithMany(a => a.Permissions)
                    .HasForeignKey(ap => ap.UserId)
                    .IsRequired();
            });
        }
    }
}
