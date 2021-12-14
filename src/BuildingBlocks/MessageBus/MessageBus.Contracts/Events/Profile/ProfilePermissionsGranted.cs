using System.Collections.Generic;

using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Events.Profile {
    public class ProfilePermissionsGranted : Message {
        public long UserId { get; set; }
        public IEnumerable<ProfilePermissionDto> Permissions { get; set; }
    }
}
