using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.VideoReaction {
    public class VideoReaction : Entity, IAggregateRoot {
        public long FixtureId { get; private set; }
        public long TeamId { get; private set; }
        public long AuthorId { get; private set; }
        public string AuthorUsername { get; private set; }
        public string Title { get; private set; }
        public string VideoId { get; private set; }
        public string ThumbnailUrl { get; private set; }
    }
}
