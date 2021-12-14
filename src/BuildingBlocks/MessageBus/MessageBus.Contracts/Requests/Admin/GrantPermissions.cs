using System.Collections.Generic;

using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Requests.Admin {
    public class GrantPermissions : Message {
        public long UserId { get; set; }
        public IEnumerable<ProfilePermissionDto> Permissions { get; set; }
    }
}
