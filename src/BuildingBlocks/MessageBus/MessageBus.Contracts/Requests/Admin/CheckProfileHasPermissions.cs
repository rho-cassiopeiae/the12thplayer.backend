using System.Collections.Generic;

namespace MessageBus.Contracts.Requests.Admin {
    public class CheckProfileHasPermissions : Message {
        public class ProfilePermission {
            public int Scope { get; set; }
            public int Flags { get; set; }
        }

        public long UserId { get; set; }
        public IEnumerable<ProfilePermission> Permissions { get; set; }
    }
}
