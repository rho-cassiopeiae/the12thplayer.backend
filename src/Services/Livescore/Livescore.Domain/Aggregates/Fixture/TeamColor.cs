using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public class TeamColor : ValueObject {
        public long TeamId { get; private set; }
        public string Color { get; private set; }
        
        public TeamColor(long teamId, string color) {
            TeamId = teamId;
            Color = color;
        }
    }
}
