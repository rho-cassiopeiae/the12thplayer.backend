using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Manager {
    public class Manager : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public long? TeamId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public long? BirthDate { get; private set; }
        public long? CountryId { get; private set; }
        public string ImageUrl { get; private set; }
        
        public Manager(
            long id, long? teamId, string firstName, string lastName,
            long? birthDate, long? countryId, string imageUrl
        ) {
            Id = id;
            TeamId = teamId;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            CountryId = countryId;
            ImageUrl = imageUrl;
        }

        public void ChangeImage(string imageUrl) {
            ImageUrl = imageUrl;
        }
    }
}
