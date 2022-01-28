using System.Collections.Generic;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Team {
    public class TeamActiveSeasons : Entity {
        public long TeamId { get; private set; }
        public List<long> ActiveSeasons { get; private set; }

        public TeamActiveSeasons(List<long> activeSeasons) {
            ActiveSeasons = activeSeasons;
        }
    }
}
