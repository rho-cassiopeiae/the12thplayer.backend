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
        public long PostedAt { get; private set; }
        public int Rating { get; private set; }
        
        public VideoReaction(
            long fixtureId, long teamId, long authorId, string authorUsername,
            string title, string videoId, string thumbnailUrl, long postedAt, int rating
        ) {
            FixtureId = fixtureId;
            TeamId = teamId;
            AuthorId = authorId;
            AuthorUsername = authorUsername;
            Title = title;
            VideoId = videoId;
            ThumbnailUrl = thumbnailUrl;
            PostedAt = postedAt;
            Rating = rating;
        }

        public VideoReaction(long fixtureId, long teamId, long authorId, int rating) {
            FixtureId = fixtureId;
            TeamId = teamId;
            AuthorId = authorId;
            Rating = rating;
        }

        public void SetTitle(string title) {
            Title = title;
        }

        public void SetAuthorUsername(string authorUsername) {
            AuthorUsername = authorUsername;
        }

        public void SetVideoId(string videoId) {
            VideoId = videoId;
        }

        public void SetThumbnail(string thumbnailUrl) {
            ThumbnailUrl = thumbnailUrl;
        }
    }
}
