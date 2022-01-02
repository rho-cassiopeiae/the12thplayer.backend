using System.Collections.Generic;
using System.Text.Json.Serialization;

using Livescore.Application.Common.Dto;

namespace Livescore.Application.Livescore.Worker.Common.Dto {
    public class FixtureLivescoreUpdateDto {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long StartTime { get; init; }
        public string Status { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GameTimeDto GameTime { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ScoreDto Score { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RefereeName { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<TeamColorDto> Colors { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<TeamLineupDto> Lineups { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<TeamMatchEventsDto> Events { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<TeamStatsDto> Stats { get; init; }
    }
}
