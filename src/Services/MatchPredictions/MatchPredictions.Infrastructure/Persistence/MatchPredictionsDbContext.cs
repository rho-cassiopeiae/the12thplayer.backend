using Microsoft.EntityFrameworkCore;

using MatchPredictions.Domain.Aggregates.Country;
using MatchPredictions.Domain.Aggregates.Fixture;
using MatchPredictions.Domain.Aggregates.League;
using MatchPredictions.Domain.Aggregates.Round;
using MatchPredictions.Domain.Aggregates.Team;
using MatchPredictions.Domain.Aggregates.UserPrediction;
using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;

namespace MatchPredictions.Infrastructure.Persistence {
    public class MatchPredictionsDbContext : DbContext {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }
        public DbSet<UserPrediction> UserPredictions { get; set; }

        public DbSet<ActiveSeasonRoundWithFixturesDto> ActiveSeasonRounds { get; set; }
        public DbSet<FixtureDto> ActiveFixtures { get; set; }

        public MatchPredictionsDbContext(DbContextOptions<MatchPredictionsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("match_predictions");

            modelBuilder.Entity<Country>(builder => {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Id).ValueGeneratedNever();
                builder.Property(c => c.Name).IsRequired();
                builder.Property(c => c.FlagUrl).IsRequired(false);
            });

            modelBuilder.Entity<League>(builder => {
                builder.HasKey(l => l.Id);
                builder.Property(l => l.Id).ValueGeneratedNever();
                builder.Property(l => l.Name).IsRequired();
                builder.Property(l => l.Type).IsRequired(false);
                builder.Property(l => l.IsCup).IsRequired(false);
                builder.Property(l => l.LogoUrl).IsRequired(false);

                builder.Metadata
                    .FindNavigation(nameof(League.Seasons))
                    .SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<Season>(builder => {
                builder.HasKey(s => s.Id);
                builder.Property(s => s.Id).ValueGeneratedNever();
                builder.Property(s => s.Name).IsRequired(false);
                builder
                    .HasOne<League>()
                    .WithMany(l => l.Seasons)
                    .HasForeignKey(s => s.LeagueId)
                    .IsRequired();
            });

            modelBuilder.Entity<Round>(builder => {
                builder.HasKey(r => r.Id);
                builder.Property(r => r.Id).ValueGeneratedNever();
                builder.Property(r => r.Name).IsRequired();
                builder.Property(r => r.StartDate).IsRequired(false);
                builder.Property(r => r.EndDate).IsRequired(false);
                builder.Property(r => r.IsCurrent).IsRequired();
                builder
                    .HasOne<Season>()
                    .WithMany()
                    .HasForeignKey(r => r.SeasonId)
                    .IsRequired();
            });

            modelBuilder.Entity<Team>(builder => {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Id).ValueGeneratedNever();
                builder.Property(t => t.Name).IsRequired();
                builder.Property(t => t.LogoUrl).IsRequired();
                builder
                    .HasOne<Country>()
                    .WithMany()
                    .HasForeignKey(t => t.CountryId)
                    .IsRequired();
            });

            modelBuilder.Entity<TeamActiveSeasons>(builder => {
                builder.HasKey(tas => tas.TeamId);
                builder.Property(tas => tas.ActiveSeasons).HasColumnType("jsonb").IsRequired();
                builder
                    .HasOne<Team>()
                    .WithOne(t => t.ActiveSeasons)
                    .HasForeignKey<TeamActiveSeasons>(tas => tas.TeamId)
                    .IsRequired();
            });

            modelBuilder.Entity<Fixture>(builder => {
                builder.HasKey(f => f.Id);
                builder.Property(f => f.Id).ValueGeneratedNever();
                builder.Property(f => f.StartTime).IsRequired();
                builder.Property(f => f.Status).IsRequired();
                builder.Property(f => f.GameTime).HasColumnType("jsonb").IsRequired();
                builder.Property(f => f.Score).HasColumnType("jsonb").IsRequired();
                builder
                    .HasOne<Season>()
                    .WithMany()
                    .HasForeignKey(f => f.SeasonId)
                    .IsRequired();
                builder
                    .HasOne<Round>()
                    .WithMany()
                    .HasForeignKey(f => f.RoundId)
                    .IsRequired();
                builder
                    .HasOne<Team>()
                    .WithMany()
                    .HasForeignKey(f => f.HomeTeamId)
                    .IsRequired();
                builder
                    .HasOne<Team>()
                    .WithMany()
                    .HasForeignKey(f => f.GuestTeamId)
                    .IsRequired();
            });

            modelBuilder.Entity<UserPrediction>(builder => {
                builder.HasKey(up => new { up.UserId, up.SeasonId, up.RoundId });
                builder
                    .Property(up => up.FixtureIdToScore)
                    .HasColumnType("jsonb")
                    .IsRequired()
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
                builder
                    .HasOne<Season>()
                    .WithMany()
                    .HasForeignKey(up => up.SeasonId)
                    .IsRequired();
                builder
                    .HasOne<Round>()
                    .WithMany()
                    .HasForeignKey(up => up.RoundId)
                    .IsRequired();
            });

            modelBuilder.Entity<ActiveSeasonRoundWithFixturesDto>(builder => {
                builder.HasNoKey();
                builder.Ignore(sr => sr.Fixtures);
                builder.ToView(nameof(ActiveSeasonRounds));
            });

            modelBuilder.Entity<FixtureDto>(builder => {
                builder.HasNoKey();
                builder.Property(f => f.GameTime).HasColumnType("jsonb");
                builder.Property(f => f.Score).HasColumnType("jsonb");
                builder.Ignore(f => f.PredictedHomeTeamScore);
                builder.Ignore(f => f.PredictedGuestTeamScore);
                builder.ToView(nameof(ActiveFixtures));
            });
        }
    }
}
