using System.Collections.Generic;

namespace Livescore.Application.Common.Dto {
    public class FixtureDiscussionUpdateDto {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public string DiscussionId { get; init; }
        public IEnumerable<DiscussionEntryDto> Entries { get; init; }
    }
}
