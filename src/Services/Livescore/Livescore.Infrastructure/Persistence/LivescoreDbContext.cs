using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Country;
using Livescore.Domain.Aggregates.Manager;
using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Aggregates.Venue;
using Livescore.Domain.Aggregates.League;
using Livescore.Domain.Aggregates.Player;
using Livescore.Domain.Aggregates.Fixture;
using Livescore.Application.Livescore.Fixture.Common.Dto;
using Livescore.Domain.Aggregates.PlayerRating;
using Livescore.Domain.Aggregates.UserVote;
using Livescore.Application.Team.Queries.GetTeamSquad;
using Livescore.Application.Team.Queries.GetPlayerRatingsForParticipant;

namespace Livescore.Infrastructure.Persistence {
    public class LivescoreDbContext : DbContext {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }
        public DbSet<PlayerRating> PlayerRatings { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }

        public DbSet<FixtureSummaryDto> FixtureSummaries { get; set; }
        public DbSet<FixtureFullDto> FixtureFullViews { get; set; }
        public DbSet<PlayerDto> PlayersWithCountry { get; set; }
        public DbSet<ManagerDto> ManagersWithCountry { get; set; }
        public DbSet<FixturePlayerRatingDto> FixturePlayerRatings { get; set; }

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
                builder.Property(s => s.IsCurrent).IsRequired();
                builder
                    .HasOne<League>()
                    .WithMany(l => l.Seasons)
                    .HasForeignKey(s => s.LeagueId)
                    .IsRequired();
            });

            modelBuilder.Entity<Player>(builder => {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Id).ValueGeneratedNever();
                builder.Property(p => p.FirstName).IsRequired(false);
                builder.Property(p => p.LastName).IsRequired(false);
                builder.Property(p => p.DisplayName).IsRequired(false);
                builder.Property(p => p.BirthDate).IsRequired(false);
                builder.Property(p => p.Number).IsRequired(false);
                builder.Property(p => p.Position).IsRequired(false);
                builder.Property(p => p.ImageUrl).IsRequired(false);
                builder.Property(p => p.LastLineupAt).IsRequired();
                builder
                    .HasOne<Country>()
                    .WithMany()
                    .HasForeignKey(p => p.CountryId)
                    .IsRequired(false);
                builder
                    .HasOne<Team>()
                    .WithMany()
                    .HasForeignKey(p => p.TeamId)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Fixture>(builder => {
                builder.HasKey(f => new { f.Id, f.TeamId });
                builder.Property(f => f.HomeStatus).IsRequired();
                builder.Property(f => f.StartTime).IsRequired();
                builder.Property(f => f.Status).IsRequired();
                builder.Property(f => f.GameTime)
                    .HasColumnType("jsonb")
                    .IsRequired();
                builder.Property(f => f.Score)
                    .HasColumnType("jsonb")
                    .IsRequired();
                builder.Property(f => f.RefereeName).IsRequired(false);
                builder.Property(f => f.Colors)
                    .HasColumnType("jsonb")
                    .IsRequired();
                builder.Property(f => f.Lineups)
                    .HasColumnType("jsonb")
                    .IsRequired();
                builder.Property(f => f.Events)
                    .HasColumnType("jsonb")
                    .IsRequired();
                builder.Property(f => f.Stats)
                    .HasColumnType("jsonb")
                    .IsRequired();
                builder
                    .HasOne<Team>()
                    .WithMany()
                    .HasForeignKey(f => f.TeamId)
                    .IsRequired();
                builder
                    .HasOne<Team>()
                    .WithMany()
                    .HasForeignKey(f => f.OpponentTeamId)
                    .IsRequired();
                builder
                    .HasOne<Season>()
                    .WithMany()
                    .HasForeignKey(f => f.SeasonId)
                    .IsRequired(false);
                builder
                    .HasOne<Venue>()
                    .WithMany()
                    .HasForeignKey(f => f.VenueId)
                    .IsRequired(false);
            });

            modelBuilder.Entity<FixtureSummaryDto>(builder => {
                builder.HasNoKey();
                builder.Property(f => f.GameTime).HasColumnType("jsonb");
                builder.Property(f => f.Score).HasColumnType("jsonb");
                builder.ToView(nameof(FixtureSummaries));
            });

            modelBuilder.Entity<FixtureFullDto>(builder => {
                builder.HasNoKey();
                builder.Property(f => f.GameTime).HasColumnType("jsonb");
                builder.Property(f => f.Score).HasColumnType("jsonb");
                builder.Property(f => f.Colors).HasColumnType("jsonb");
                builder.Property(f => f.Lineups).HasColumnType("jsonb");
                builder.Property(f => f.Events).HasColumnType("jsonb");
                builder.Property(f => f.Stats).HasColumnType("jsonb");
                builder.ToView(nameof(FixtureFullViews));
            });

            modelBuilder.Entity<PlayerRating>(builder => {
                builder.HasKey(pr => new { pr.FixtureId, pr.TeamId, pr.ParticipantKey });
                builder.Property(pr => pr.TotalRating).IsRequired();
                builder.Property(pr => pr.TotalVoters).IsRequired();
                builder
                    .HasOne<Fixture>()
                    .WithMany()
                    .HasForeignKey(pr => new { pr.FixtureId, pr.TeamId })
                    .IsRequired();
            });

            modelBuilder.Entity<UserVote>(builder => {
                builder.HasKey(uv => new { uv.UserId, uv.FixtureId, uv.TeamId });
                builder
                    .HasOne<Fixture>()
                    .WithMany()
                    .HasForeignKey(uv => new { uv.FixtureId, uv.TeamId })
                    .IsRequired();
                builder
                    .Property(uv => uv.FixtureParticipantKeyToRating)
                    .HasColumnType("jsonb")
                    .IsRequired(false)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
                builder
                    .Property(uv => uv.LiveCommentaryAuthorIdToVote)
                    .HasColumnType("jsonb")
                    .IsRequired(false)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
                builder
                    .Property(uv => uv.VideoReactionAuthorIdToVote)
                    .HasColumnType("jsonb")
                    .IsRequired(false)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<PlayerDto>(builder => {
                builder.HasNoKey();
                builder.ToView(nameof(PlayersWithCountry));
            });

            modelBuilder.Entity<ManagerDto>(builder => {
                builder.HasNoKey();
                builder.ToView(nameof(ManagersWithCountry));
            });

            modelBuilder.Entity<FixturePlayerRatingDto>(builder => {
                builder.HasNoKey();
                builder.ToView(nameof(FixturePlayerRatings));
            });
        }
    }
}
