using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Country;
using Livescore.Domain.Aggregates.Manager;
using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Aggregates.Venue;

namespace Livescore.Infrastructure.Persistence {
    public class LivescoreDbContext : DbContext {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Manager> Managers { get; set; }

        public LivescoreDbContext(DbContextOptions<LivescoreDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("livescore");

            modelBuilder.Entity<Country>(builder => {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Id).ValueGeneratedNever();
                builder.Property(c => c.Name).IsRequired();
                builder.Property(c => c.FlagUrl).IsRequired(false);
            });
            
            modelBuilder.Entity<Team>(builder => {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Id).ValueGeneratedNever();
                builder.Property(t => t.Name).IsRequired();
                builder.Property(t => t.LogoUrl).IsRequired();
                builder.Property(t => t.HasThe12thPlayerCommunity).IsRequired();
                builder
                    .HasOne<Country>()
                    .WithMany()
                    .HasForeignKey(t => t.CountryId)
                    .IsRequired();
            });

            modelBuilder.Entity<Venue>(builder => {
                builder.HasKey(v => v.Id);
                builder.Property(v => v.Id).ValueGeneratedNever();
                builder.Property(v => v.Name).IsRequired();
                builder.Property(v => v.City).IsRequired(false);
                builder.Property(v => v.Capacity).IsRequired(false);
                builder.Property(v => v.ImageUrl).IsRequired(false);
                builder
                    .HasOne<Team>()
                    .WithOne()
                    .HasForeignKey<Venue>(v => v.TeamId)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Manager>(builder => {
                builder.HasKey(m => m.Id);
                builder.Property(m => m.Id).ValueGeneratedNever();
                builder.Property(m => m.FirstName).IsRequired(false);
                builder.Property(m => m.LastName).IsRequired(false);
                builder.Property(m => m.BirthDate).IsRequired(false);
                builder.Property(m => m.ImageUrl).IsRequired(false);
                builder
                    .HasOne<Country>()
                    .WithMany()
                    .HasForeignKey(m => m.CountryId)
                    .IsRequired(false);
                builder
                    .HasOne<Team>()
                    .WithOne()
                    .HasForeignKey<Manager>(m => m.TeamId)
                    .IsRequired(false);
            });
        }
    }
}
