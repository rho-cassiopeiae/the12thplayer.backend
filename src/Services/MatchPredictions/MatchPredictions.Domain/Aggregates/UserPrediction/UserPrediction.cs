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

        public void SetPredictions(Dictionary<string, string> predictions) {
            _fixtureIdToScore = predictions;
        }

        public void AddPredictions(Dictionary<string, string> predictions) {
            // @@NOTE: _fixtureIdToScore can't be null here, since it's configured as non-nullable in the db.
            // @@??: Create a copy of the current predictions dictionary and assign it to the field, so that ChangeTracker
            // picks up the update. Have to do it ?
            _fixtureIdToScore = new(_fixtureIdToScore);
            foreach (var prediction in predictions) {
                _fixtureIdToScore[prediction.Key] = prediction.Value; // add new or overwrite old
            }
        }
    }
}
