using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using NpgsqlTypes;

using MatchPredictions.Application.Playtime.Queries.GetActiveFixturesForTeam;
using MatchPredictions.Application.Common.Interfaces;
using MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions;
using MatchPredictions.Application.Common.Dto;

namespace MatchPredictions.Infrastructure.Persistence.Queryables {
    public class FixtureQueryable : IFixtureQueryable {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public FixtureQueryable(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task<IEnumerable<FixtureDto>> GetAllFor(
            IEnumerable<long> seasonIds, IEnumerable<long> roundIds
        ) {
            var seasonIdsParam = new NpgsqlParameter<long[]>(nameof(seasonIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = seasonIds.ToArray()
            };
            var roundIdsParam = new NpgsqlParameter<long[]>(nameof(roundIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = roundIds.ToArray()
            };

            var fixtures = await _matchPredictionsDbContext.ActiveFixtures
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM match_predictions.get_fixtures_by_season_and_round_id(
                            @{seasonIdsParam.ParameterName}, @{roundIdsParam.ParameterName}
                        )
                    ",
                    seasonIdsParam, roundIdsParam
                )
                .ToListAsync();

            return fixtures;
        }

        public async Task<IEnumerable<NotStartedFixtureDto>> GetOnlyNotStarted(IEnumerable<long> fixtureIds) {
            var fixtureIdsParam = new NpgsqlParameter<long[]>(nameof(fixtureIds), NpgsqlDbType.Array | NpgsqlDbType.Bigint) {
                TypedValue = fixtureIds.ToArray()
            };

            var notStartedFixtures = await _matchPredictionsDbContext.NotStartedFixtures
                .FromSqlRaw(
                    $@"
                        SELECT *
                        FROM match_predictions.filter_out_already_started_fixtures(@{fixtureIdsParam.ParameterName})
                    ",
                    fixtureIdsParam
                )
                .ToListAsync();

            return notStartedFixtures;
        }

        public async Task<IEnumerable<AlreadyStartedFixtureDto>> GetById(IEnumerable<long> fixtureIds) {
            var fixtures = await _matchPredictionsDbContext.Fixtures
                .Where(f => fixtureIds.Contains(f.Id))
                .Select(f => new AlreadyStartedFixtureDto {
                    Id = f.Id,
                    StartTime = f.StartTime,
                    Status = f.Status,
                    GameTime = new GameTimeDto {
                        Minute = f.GameTime.Minute,
                        ExtraTimeMinute = f.GameTime.ExtraTimeMinute,
                        AddedTimeMinute = f.GameTime.AddedTimeMinute
                    },
                    Score = new ScoreDto {
                        LocalTeam = f.Score.LocalTeam,
                        VisitorTeam = f.Score.VisitorTeam,
                        HT = f.Score.HT,
                        FT = f.Score.FT,
                        ET = f.Score.ET,
                        PS = f.Score.PS
                    }
                })
                .ToListAsync();

            return fixtures;
        }
    }
}
