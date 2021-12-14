using System.Collections.Generic;

using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Responses.Profile {
    public class GetProfilePermissionsSuccess : Message {
        public IEnumerable<ProfilePermissionDto> Permissions { get; set; }
    }
}
