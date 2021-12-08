using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Respawn;
using MediatR;
using StackExchange.Redis;

using Livescore.Api;
using Livescore.Infrastructure.Persistence;
using Livescore.Application.Seed.Commands.AddCountries;
using Livescore.Application.Common.Dto;
using Livescore.Application.Seed.Commands.AddTeamDetails;
using Livescore.Application.Seed.Commands.AddTeamUpcomingFixtures;
using Livescore.Application.Common.Interfaces;

namespace Livescore.IntegrationTests {
    public class Sut {
        private readonly IHost _host;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly Checkpoint _checkpoint;

        private ClaimsPrincipal _user;

        public Sut() {
            _host = Program
                .CreateHostBuilder(args: null)
                .ConfigureAppConfiguration((hostContext, builder) => {
                    builder.AddJsonFile("appsettings.Testing.json", optional: false);
                })
                .Build();

            _hostEnvironment = _host.Services.GetRequiredService<IHostEnvironment>();

            _checkpoint = new Checkpoint {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] {
                    "livescore"
                },
                TablesToIgnore = new[] {
                    "__EFMigrationsHistory_LivescoreDbContext"
                }
            };

            _applyMigrations();
        }

        private void _applyMigrations() {
            using var scope = _host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<LivescoreDbContext>();
            context.Database.Migrate();
        }

        public void ResetState() {
            using var scope = _host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<LivescoreDbContext>();
            context.Database.OpenConnection();
            _checkpoint.Reset(context.Database.GetDbConnection()).Wait();

            var redis = scope.ServiceProvider.GetRequiredService<ConnectionMultiplexer>();
            redis.GetServer(redis.Configuration.Split(',').First()).FlushDatabase();

            RunAsGuest();
        }

        public void RunAs(long userId, string username) {
            _user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim("__Username", username)
                },
                "Bearer"
            ));
        }

        public void RunAsGuest() {
            _user = null;
        }

        private string _getFileContent(string path) {
            var fileInfo = _hostEnvironment.ContentRootFileProvider.GetFileInfo(path);
            if (fileInfo.Exists) {
                using var reader = new StreamReader(fileInfo.CreateReadStream());
                return reader.ReadToEnd();
            }

            throw new FileNotFoundException();
        }

        public (long FixtureId, long TeamId) SeedWithDummyUpcomingFixture() {
            SendRequest(
                new AddCountriesCommand {
                    Countries = JsonSerializer.Deserialize<IEnumerable<CountryDto>>(
                        _getFileContent("DummyData/countries.json")
                    )
                }
            ).Wait();

            var addTeamDetailsCommand = new AddTeamDetailsCommand {
                Team = JsonSerializer.Deserialize<TeamDto>(
                    _getFileContent("DummyData/team-details.json")
                )
            };
            SendRequest(addTeamDetailsCommand).Wait();

            var fixture = JsonSerializer.Deserialize<IEnumerable<FixtureDto>>(
                _getFileContent("DummyData/finished-fixtures.json")
            ).First();

            fixture.Status = "NS";
            fixture.GameTime = new GameTimeDto();
            fixture.Score = new ScoreDto();
            fixture.RefereeName = null;
            fixture.Colors = new[] {
                new TeamColorDto {
                    TeamId = addTeamDetailsCommand.Team.Id
                },
                new TeamColorDto {
                    TeamId = fixture.OpponentTeam.Id
                }
            };
            fixture.Lineups = new[] {
                new TeamLineupDto {
                    TeamId = addTeamDetailsCommand.Team.Id,
                    StartingXI = new TeamLineupDto.PlayerDto[] { },
                    Subs = new TeamLineupDto.PlayerDto[] { }
                },
                new TeamLineupDto {
                    TeamId = fixture.OpponentTeam.Id,
                    StartingXI = new TeamLineupDto.PlayerDto[] { },
                    Subs = new TeamLineupDto.PlayerDto[] { }
                }
            };
            fixture.Events = new[] {
                new TeamMatchEventsDto {
                    TeamId = addTeamDetailsCommand.Team.Id,
                    Events = new TeamMatchEventsDto.MatchEventDto[] { }
                },
                new TeamMatchEventsDto {
                    TeamId = fixture.OpponentTeam.Id,
                    Events = new TeamMatchEventsDto.MatchEventDto[] { }
                }
            };
            fixture.Stats = new[] {
                new TeamStatsDto {
                    TeamId = addTeamDetailsCommand.Team.Id
                },
                new TeamStatsDto {
                    TeamId = fixture.OpponentTeam.Id
                }
            };

            SendRequest(
                new AddTeamUpcomingFixturesCommand {
                    TeamId = addTeamDetailsCommand.Team.Id,
                    Fixtures = new[] { fixture },
                    Seasons = JsonSerializer.Deserialize<IEnumerable<SeasonDto>>(
                        _getFileContent("DummyData/seasons.json")
                    )
                }
            ).Wait();

            return (fixture.Id, addTeamDetailsCommand.Team.Id);
        }

        public FixtureDto GetSeededFixtureWithDummyPrematchData() {
            var fixture = JsonSerializer.Deserialize<IEnumerable<FixtureDto>>(
                _getFileContent("DummyData/finished-fixtures.json")
            ).First();

            fixture.Status = "NS";

            return fixture;
        }

        public async Task<T> SendRequest<T>(IRequest<T> request) {
            using var scope = _host.Services.CreateScope();

            if (_user != null) {
                var context = scope.ServiceProvider.GetRequiredService<IAuthenticationContext>();
                context.User = _user;
            }

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await mediator.Send(request);

            return result;
        }
    }
}
