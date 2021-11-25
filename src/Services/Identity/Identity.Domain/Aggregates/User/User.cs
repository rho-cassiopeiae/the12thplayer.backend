using Identity.Domain.Base;

namespace Identity.Domain.Aggregates.User {
    public class User : IAggregateRoot {
        public long Id { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        
        public User(string email, string username) {
            Email = email;
            Username = username;
        }
    }
}
