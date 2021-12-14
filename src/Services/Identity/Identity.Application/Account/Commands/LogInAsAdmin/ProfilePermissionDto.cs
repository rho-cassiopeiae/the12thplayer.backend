namespace Identity.Application.Account.Commands.LogInAsAdmin {
    public class ProfilePermissionDto {
        public PermissionScope Scope { get; init; }
        public int Flags { get; init; }
    }
}
