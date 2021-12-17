using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Author {
    public class AuthorPermission : Entity {
        public long UserId { get; private set; }
        public PermissionScope Scope { get; private set; }
        public short Flags { get; private set; }

        public AuthorPermission(PermissionScope scope, short flags) {
            Scope = scope;
            Flags = flags;
        }

        internal void AddFlags(short flags) {
            Flags |= flags;
        }
    }
}
