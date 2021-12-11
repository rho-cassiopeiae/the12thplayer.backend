using System.Collections.Generic;

namespace Livescore.Infrastructure.InMemory.Listeners.FixtureDiscussionListener {
    public class FixtureDiscussionUpdateDto {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public string DiscussionId { get; init; }
        public IEnumerable<DiscussionEntryDto> Entries { get; init; }
    }
}
