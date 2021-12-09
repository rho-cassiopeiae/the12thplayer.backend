﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Livescore.Application.Common.Dto;
using Livescore.Domain.Aggregates.Fixture;
using Livescore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Livescore.Infrastructure.Persistence.Migrations.Livescore
{
    [DbContext(typeof(LivescoreDbContext))]
    [Migration("20211208144229_Add_Player_Rating")]
    partial class Add_Player_Rating
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("livescore")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Livescore.Application.Livescore.Fixture.Common.Dto.FixtureFullDto", b =>
                {
                    b.Property<IEnumerable<TeamColorDto>>("Colors")
                        .HasColumnType("jsonb");

                    b.Property<IEnumerable<TeamMatchEventsDto>>("Events")
                        .HasColumnType("jsonb");

                    b.Property<GameTimeDto>("GameTime")
                        .HasColumnType("jsonb");

                    b.Property<bool>("HomeStatus")
                        .HasColumnType("boolean");

                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("LeagueLogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("LeagueName")
                        .HasColumnType("text");

                    b.Property<IEnumerable<TeamLineupDto>>("Lineups")
                        .HasColumnType("jsonb");

                    b.Property<long>("OpponentTeamId")
                        .HasColumnType("bigint");

                    b.Property<string>("OpponentTeamLogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("OpponentTeamName")
                        .HasColumnType("text");

                    b.Property<string>("RefereeName")
                        .HasColumnType("text");

                    b.Property<ScoreDto>("Score")
                        .HasColumnType("jsonb");

                    b.Property<long?>("StartTime")
                        .HasColumnType("bigint");

                    b.Property<IEnumerable<TeamStatsDto>>("Stats")
                        .HasColumnType("jsonb");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("VenueImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("VenueName")
                        .HasColumnType("text");

                    b.ToView("FixtureFullViews");
                });

            modelBuilder.Entity("Livescore.Application.Livescore.Fixture.Common.Dto.FixtureSummaryDto", b =>
                {
                    b.Property<GameTimeDto>("GameTime")
                        .HasColumnType("jsonb");

                    b.Property<bool>("HomeStatus")
                        .HasColumnType("boolean");

                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("LeagueLogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("LeagueName")
                        .HasColumnType("text");

                    b.Property<long>("OpponentTeamId")
                        .HasColumnType("bigint");

                    b.Property<string>("OpponentTeamLogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("OpponentTeamName")
                        .HasColumnType("text");

                    b.Property<ScoreDto>("Score")
                        .HasColumnType("jsonb");

                    b.Property<long?>("StartTime")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("VenueImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("VenueName")
                        .HasColumnType("text");

                    b.ToView("FixtureSummaries");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Country.Country", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("FlagUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Fixture.Fixture", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamId")
                        .HasColumnType("bigint");

                    b.Property<IEnumerable<TeamColor>>("Colors")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<IEnumerable<TeamMatchEvents>>("Events")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<GameTime>("GameTime")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<bool>("HomeStatus")
                        .HasColumnType("boolean");

                    b.Property<IEnumerable<TeamLineup>>("Lineups")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long>("OpponentTeamId")
                        .HasColumnType("bigint");

                    b.Property<string>("RefereeName")
                        .HasColumnType("text");

                    b.Property<Score>("Score")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<long?>("SeasonId")
                        .HasColumnType("bigint");

                    b.Property<long?>("StartTime")
                        .HasColumnType("bigint");

                    b.Property<IEnumerable<TeamStats>>("Stats")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("VenueId")
                        .HasColumnType("bigint");

                    b.HasKey("Id", "TeamId");

                    b.HasIndex("OpponentTeamId");

                    b.HasIndex("SeasonId");

                    b.HasIndex("TeamId");

                    b.HasIndex("VenueId");

                    b.ToTable("Fixtures");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.League.League", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool?>("IsCup")
                        .HasColumnType("boolean");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.League.Season", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsCurrent")
                        .HasColumnType("boolean");

                    b.Property<long>("LeagueId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LeagueId");

                    b.ToTable("Season");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Manager.Manager", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long?>("BirthDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("CountryId")
                        .HasColumnType("bigint");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<long?>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("TeamId")
                        .IsUnique();

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Player.Player", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long?>("BirthDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("CountryId")
                        .HasColumnType("bigint");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<long>("LastLineupAt")
                        .HasColumnType("bigint");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<short?>("Number")
                        .HasColumnType("smallint");

                    b.Property<string>("Position")
                        .HasColumnType("text");

                    b.Property<long?>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("TeamId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.PlayerRating.PlayerRating", b =>
                {
                    b.Property<long>("FixtureId")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamId")
                        .HasColumnType("bigint");

                    b.Property<string>("ParticipantKey")
                        .HasColumnType("text");

                    b.Property<int>("TotalRating")
                        .HasColumnType("integer");

                    b.Property<int>("TotalVoters")
                        .HasColumnType("integer");

                    b.HasKey("FixtureId", "TeamId", "ParticipantKey");

                    b.ToTable("PlayerRatings");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Team.Team", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("CountryId")
                        .HasColumnType("bigint");

                    b.Property<bool>("HasThe12thPlayerCommunity")
                        .HasColumnType("boolean");

                    b.Property<string>("LogoUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Venue.Venue", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int?>("Capacity")
                        .HasColumnType("integer");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TeamId")
                        .IsUnique();

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Fixture.Fixture", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.Team.Team", null)
                        .WithMany()
                        .HasForeignKey("OpponentTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Livescore.Domain.Aggregates.League.Season", null)
                        .WithMany()
                        .HasForeignKey("SeasonId");

                    b.HasOne("Livescore.Domain.Aggregates.Team.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Livescore.Domain.Aggregates.Venue.Venue", null)
                        .WithMany()
                        .HasForeignKey("VenueId");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.League.Season", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.League.League", null)
                        .WithMany("Seasons")
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Manager.Manager", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.Country.Country", null)
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("Livescore.Domain.Aggregates.Team.Team", null)
                        .WithOne()
                        .HasForeignKey("Livescore.Domain.Aggregates.Manager.Manager", "TeamId");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Player.Player", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.Country.Country", null)
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("Livescore.Domain.Aggregates.Team.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.PlayerRating.PlayerRating", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.Fixture.Fixture", null)
                        .WithMany()
                        .HasForeignKey("FixtureId", "TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Team.Team", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.Country.Country", null)
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.Venue.Venue", b =>
                {
                    b.HasOne("Livescore.Domain.Aggregates.Team.Team", null)
                        .WithOne()
                        .HasForeignKey("Livescore.Domain.Aggregates.Venue.Venue", "TeamId");
                });

            modelBuilder.Entity("Livescore.Domain.Aggregates.League.League", b =>
                {
                    b.Navigation("Seasons");
                });
#pragma warning restore 612, 618
        }
    }
}
