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

        public float? ChangePlayerRating(string participantKey, float rating) {
            float? currentRating = _fixtureParticipantKeyToRating[participantKey];
            _fixtureParticipantKeyToRating[participantKey] = rating;
            
            return currentRating;
        }

        public void AddVideoReactionVote(long authorId, short? vote) {
            _videoReactionAuthorIdToVote ??= new Dictionary<string, short?>();
            _videoReactionAuthorIdToVote[authorId.ToString()] = vote;
        }

        public (short? OldVote, int IncrementRatingBy) ChangeVideoReactionVote(long authorId, short? vote) {
            var key = authorId.ToString();

            short? currentVote = _videoReactionAuthorIdToVote[key];
            int incrementRatingBy;
            if (currentVote == null) {
                incrementRatingBy = vote.Value;
            } else if (currentVote == 1) {
                if (vote == 1) {
                    incrementRatingBy = -1;
                    vote = null;
                } else {
                    incrementRatingBy = -2;
                }
            } else {
                if (vote == -1) {
                    incrementRatingBy = 1;
                    vote = null;
                } else {
                    incrementRatingBy = 2;
                }
            }

            _videoReactionAuthorIdToVote[key] = vote;

            return (currentVote, incrementRatingBy);
        }
    }
}
