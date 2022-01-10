using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.UserVote {
    public class UserVote : Entity, IAggregateRoot {
        public long UserId { get; private set; }
        public long FixtureId { get; private set; }
        public long TeamId { get; private set; }

        private Dictionary<string, float?> _fixtureParticipantKeyToRating;
        public IReadOnlyDictionary<string, float?> FixtureParticipantKeyToRating =>
            _fixtureParticipantKeyToRating;

        private Dictionary<string, short?> _liveCommentaryAuthorIdToVote;
        public IReadOnlyDictionary<string, short?> LiveCommentaryAuthorIdToVote =>
            _liveCommentaryAuthorIdToVote;

        private Dictionary<string, short?> _videoReactionAuthorIdToVote;
        public IReadOnlyDictionary<string, short?> VideoReactionAuthorIdToVote =>
            _videoReactionAuthorIdToVote;

        public UserVote(long userId, long fixtureId, long teamId) {
            UserId = userId;
            FixtureId = fixtureId;
            TeamId = teamId;
        }

        public void AddPlayerRating(string participantKey, float? rating) {
            _fixtureParticipantKeyToRating ??= new Dictionary<string, float?>();
            _fixtureParticipantKeyToRating[participantKey] = rating;
        }

        public void SetPlayerRatings(IReadOnlyDictionary<string, float?> playerRatings) {
            _fixtureParticipantKeyToRating = new Dictionary<string, float?>(playerRatings);
        }

        public float? ChangePlayerRating(string participantKey, float rating) {
            float? currentRating = _fixtureParticipantKeyToRating[participantKey];
            _fixtureParticipantKeyToRating[participantKey] = rating;
            
            return currentRating;
        }

        public void AddVideoReactionVote(long authorId, short? userVote) {
            _videoReactionAuthorIdToVote ??= new Dictionary<string, short?>();
            _videoReactionAuthorIdToVote[authorId.ToString()] = userVote;
        }

        public (short? OldVote, int IncrementRatingBy) ChangeVideoReactionVote(long authorId, short? userVote) {
            var key = authorId.ToString();
            short? currentVote = _videoReactionAuthorIdToVote[key];
            int incrementRatingBy = userVote.GetValueOrDefault() - currentVote.GetValueOrDefault();
            _videoReactionAuthorIdToVote[key] = userVote;

            return (currentVote, incrementRatingBy);
        }
    }
}
