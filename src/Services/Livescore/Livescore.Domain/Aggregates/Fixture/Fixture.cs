using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public class Fixture : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public long TeamId { get; private set; }
        public long? SeasonId { get; private set; }
        public long OpponentTeamId { get; private set; }
        public bool HomeStatus { get; private set; }
        public long StartTime { get; private set; }
        public string Status { get; private set; }
        public GameTime GameTime { get; private set; }
        public Score Score { get; private set; }
        public long? VenueId { get; private set; }
        public string RefereeName { get; private set; }
        public IEnumerable<TeamColor> Colors { get; private set; }
        public IEnumerable<TeamLineup> Lineups { get; private set; }
        public IEnumerable<TeamMatchEvents> Events { get; private set; }
        public IEnumerable<TeamStats> Stats { get; private set; }

        public Fixture(
            long id, long teamId, long? seasonId, long opponentTeamId,
            bool homeStatus, long startTime, string status, GameTime gameTime,
            Score score, long? venueId, string refereeName, IEnumerable<TeamColor> colors,
            IEnumerable<TeamLineup> lineups, IEnumerable<TeamMatchEvents> events, IEnumerable<TeamStats> stats
        ) {
            Id = id;
            TeamId = teamId;
            SeasonId = seasonId;
            OpponentTeamId = opponentTeamId;
            HomeStatus = homeStatus;
            StartTime = startTime;
            Status = status;
            GameTime = gameTime;
            Score = score;
            VenueId = venueId;
            RefereeName = refereeName;
            Colors = colors;
            Lineups = lineups;
            Events = events;
            Stats = stats;
        }

        public void SetStartTime(long startTime) {
            StartTime = startTime;
        }

        public void SetStatus(string status) {
            Status = status;
        }

        public void SetGameTime(GameTime gameTime) {
            GameTime = gameTime;
        }

        public void SetScore(Score score) {
            Score = score;
        }

        public void SetReferee(string refereeName) {
            RefereeName = refereeName;
        }

        public void SetColors(IEnumerable<TeamColor> colors) {
            Colors = colors;
        }

        public void SetLineups(IEnumerable<TeamLineup> lineups) {
            Lineups = lineups;
        }

        public void SetEvents(IEnumerable<TeamMatchEvents> events) {
            Events = events;
        }

        public void SetStats(IEnumerable<TeamStats> stats) {
            Stats = stats;
        }
    }
}
