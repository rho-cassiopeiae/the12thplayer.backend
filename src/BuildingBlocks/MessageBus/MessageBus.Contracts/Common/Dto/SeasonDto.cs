using System.Collections.Generic;

namespace MessageBus.Contracts.Common.Dto {
    public class SeasonDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? CurrentRoundId { get; set; }
        public LeagueDto League { get; set; }
        public IEnumerable<RoundDto> Rounds { get; set; }
    }
}
