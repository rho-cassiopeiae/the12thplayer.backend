using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Discussion {
    public class DiscussionEntry : Entity {
        public string Id { get; private set; }
        public long UserId { get; private set; }
        public string Username { get; private set; }
        public string Body { get; private set; }
        
        public DiscussionEntry(string id, long userId, string username, string body) {
            Id = id;
            UserId = userId;
            Username = username;
            Body = body;
        }
    }
}
