using System.Collections.Generic;

namespace MessageBus.Contracts.Responses.Profile {
    public class CollectProfilePermissionsSuccess : Message {
        public class ProfilePermission {
            public int Scope { get; set; }
            public int Flags { get; set; }
        }

        public IEnumerable<ProfilePermission> Permissions { get; set; }
    }
}
