using Profile.Domain.Base;

namespace Profile.Domain.Aggregates.Profile {
    public class ProfilePermission : Entity {
        public long UserId { get; private set; }
        public PermissionScope Scope { get; private set; }
        public int Flags { get; private set; }

        public ProfilePermission(PermissionScope scope, int flags) {
            Scope = scope;
            Flags = flags;
        }

        internal void AddFlags(int flags) {
            Flags |= flags;
        }
    }
}
