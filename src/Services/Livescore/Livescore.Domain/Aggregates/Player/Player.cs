using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Player {
    public class Player : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public long? TeamId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public long? BirthDate { get; private set; }
        public long? CountryId { get; private set; }
        public short? Number { get; private set; }
        public string Position { get; private set; }
        public string ImageUrl { get; private set; }
        public long LastLineupAt { get; private set; }
        
        public Player(
            long id, long? teamId, string firstName,
            string lastName, long? birthDate, long? countryId,
            short? number, string position, string imageUrl,
            long lastLineupAt
        ) {
            Id = id;
            TeamId = teamId;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            CountryId = countryId;
            Number = number;
            Position = position;
            ImageUrl = imageUrl;
            LastLineupAt = lastLineupAt;
        }

        public void ChangeTeam(long teamId) {
            TeamId = teamId;
        }

        public void ChangeNumber(short number) {
            Number = number;
        }

        public void SetPosition(string position) {
            Position = position;
        }

        public void SetLastLineupAt(long lastLineupAt) {
            LastLineupAt = lastLineupAt;
        }
    }
}
