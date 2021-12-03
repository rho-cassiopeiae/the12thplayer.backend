using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Venue {
    public class Venue : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public long? TeamId { get; private set; }
        public string Name { get; private set; }
        public string City { get; private set; }
        public int? Capacity { get; private set; }
        public string ImageUrl { get; private set; }
        
        public Venue(
            long id, long? teamId, string name,
            string city, int? capacity, string imageUrl
        ) {
            Id = id;
            TeamId = teamId;
            Name = name;
            City = city;
            Capacity = capacity;
            ImageUrl = imageUrl;
        }

        public void ChangeName(string name) {
            Name = name;
        }

        public void ChangeCapacity(int? capacity) {
            Capacity = capacity;
        }

        public void ChangeImage(string imageUrl) {
            ImageUrl = imageUrl;
        }
    }
}
