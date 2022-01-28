using System.Collections.Generic;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.UserPrediction {
    public class UserPrediction : Entity, IAggregateRoot {
        public long UserId { get; private set; }
        public long SeasonId { get; private set; }
        public long RoundId { get; private set; }

        private Dictionary<string, string> _fixtureIdToScore;
        public IReadOnlyDictionary<string, string> FixtureIdToScore => _fixtureIdToScore;

        public UserPrediction(long userId, long seasonId, long roundId) {
            UserId = userId;
            SeasonId = seasonId;
            RoundId = roundId;
        }
    }
}
