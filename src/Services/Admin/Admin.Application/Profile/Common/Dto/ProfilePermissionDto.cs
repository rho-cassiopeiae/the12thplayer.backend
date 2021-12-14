using Admin.Application.Common.Enums;

namespace Admin.Application.Profile.Common.Dto {
    public class ProfilePermissionDto {
        public PermissionScope Scope { get; init; }
        public int Flags { get; init; }
    }
}
