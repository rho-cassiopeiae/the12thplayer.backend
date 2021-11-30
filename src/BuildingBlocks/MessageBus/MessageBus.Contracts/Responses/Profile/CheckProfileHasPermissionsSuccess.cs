namespace MessageBus.Contracts.Responses.Profile {
    public class CheckProfileHasPermissionsSuccess : Message {
        public bool HasRequiredPermissions { get; set; }
    }
}
