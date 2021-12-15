using Microsoft.EntityFrameworkCore;

using Feed.Domain.Aggregates.Author;
using Feed.Domain.Aggregates.Article;
using Feed.Domain.Aggregates.UserVote;

namespace Feed.Infrastructure.Persistence {
    public class FeedDbContext : DbContext {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }

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

            modelBuilder.Entity<Article>(builder => {
                builder.HasKey(a => a.Id);
                builder.Property(a => a.TeamId).IsRequired();
                builder.Property(a => a.AuthorUsername).IsRequired();
                builder.Property(a => a.PostedAt).IsRequired();
                builder.Property(a => a.Type).IsRequired();
                builder.Property(a => a.Title).IsRequired();
                builder.Property(a => a.PreviewImageUrl).IsRequired(false);
                builder.Property(a => a.Summary).IsRequired(false);
                builder.Property(a => a.Content).IsRequired();
                builder.Property(a => a.Rating).IsRequired();
                builder
                    .HasOne<Author>()
                    .WithMany()
                    .HasForeignKey(a => a.AuthorId)
                    .IsRequired();
            });

            modelBuilder.Entity<UserVote>(builder => {
                builder.HasKey(uv => new { uv.UserId, uv.ArticleId });
                builder.Property(uv => uv.ArticleVote).IsRequired(false);
                builder
                    .Property(uv => uv.CommentIdToVote)
                    .HasColumnType("jsonb")
                    .IsRequired(false)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
                builder
                    .HasOne<Author>()
                    .WithMany()
                    .HasForeignKey(uv => uv.UserId)
                    .IsRequired();
                builder
                    .HasOne<Article>()
                    .WithMany()
                    .HasForeignKey(uv => uv.ArticleId)
                    .IsRequired();
                builder.Property<short?>("OldVote");
            });
        }
    }
}
