using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MatchPredictions.Application.Playtime.Commands.SubmitMatchPredictions {
    public class MatchPredictionsSubmissionDto {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, string> UpdatedPredictions { get; init; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<AlreadyStartedFixtureDto> AlreadyStartedFixtures { get; init; }
    }
}
