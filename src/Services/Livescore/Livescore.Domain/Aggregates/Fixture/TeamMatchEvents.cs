using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public class TeamMatchEvents : ValueObject {
        public class MatchEvent {
            public short Minute { get; private set; }
            public short? AddedTimeMinute { get; private set; }
            public string Type { get; private set; }
            public long? PlayerId { get; private set; }
            public long? RelatedPlayerId { get; private set; }
            
            public MatchEvent(
                short minute, short? addedTimeMinute, string type,
                long? playerId, long? relatedPlayerId
            ) {
                Minute = minute;
                AddedTimeMinute = addedTimeMinute;
                Type = type;
                PlayerId = playerId;
                RelatedPlayerId = relatedPlayerId;
            }
        }

        public long TeamId { get; private set; }
        public IEnumerable<MatchEvent> Events { get; private set; }

        public TeamMatchEvents(long teamId, IEnumerable<MatchEvent> events) {
            TeamId = teamId;
            Events = events;
        }
    }
}
