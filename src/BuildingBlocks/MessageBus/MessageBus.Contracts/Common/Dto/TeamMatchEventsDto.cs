using System.Collections.Generic;

namespace MessageBus.Contracts.Common.Dto {
    public class TeamMatchEventsDto {
        public class MatchEventDto {
            public short Minute { get; set; }
            public short? AddedTimeMinute { get; set; }
            public string Type { get; set; }
            public long? PlayerId { get; set; }
            public long? RelatedPlayerId { get; set; }
        }

        public long TeamId { get; set; }
        public IEnumerable<MatchEventDto> Events { get; set; }
    }
}
