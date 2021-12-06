using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.PlayerRating {
    public class PlayerRating : Entity, IAggregateRoot {
        public long FixtureId { get; private set; }
        public long TeamId { get; private set; }
        public string ParticipantKey { get; private set; }
        public int TotalRating { get; private set; }
        public int TotalVoters { get; private set; }

        private List<UserVote> _userVotes = new();
        public IReadOnlyList<UserVote> UserVotes => _userVotes;
        
        public PlayerRating(
            long fixtureId, long teamId, string participantKey,
            int totalRating, int totalVoters
        ) {
            FixtureId = fixtureId;
            TeamId = teamId;
            ParticipantKey = participantKey;
            TotalRating = totalRating;
            TotalVoters = totalVoters;
        }

        public PlayerRating(long fixtureId, long teamId, string participantKey) {
            FixtureId = fixtureId;
            TeamId = teamId;
            ParticipantKey = participantKey;
        }

        public void AddUserVote(UserVote userVote) {
            _userVotes.Add(userVote);
        }

        public void RemoveUserVote(UserVote userVote) {
            _userVotes.Remove(userVote);
        }
    }
}
