using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Author {
    public class AuthorPermission : Entity {
        public long UserId { get; private set; }
        public PermissionScope Scope { get; private set; }
        public int Flags { get; private set; }

        public AuthorPermission(PermissionScope scope, int flags) {
            Scope = scope;
            Flags = flags;
        }

        internal void AddFlags(int flags) {
            Flags |= flags;
        }
    }
}
