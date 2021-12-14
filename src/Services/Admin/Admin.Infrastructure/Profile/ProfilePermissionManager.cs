using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Responses.Profile;
using ProfilePermissionDtoMsg = MessageBus.Contracts.Common.Dto.ProfilePermissionDto;

using Admin.Application.Common.Interfaces;
using Admin.Application.Profile.Common.Dto;

namespace Admin.Infrastructure.Profile {
    public class ProfilePermissionManager : IProfilePermissionManager {
        private readonly IBus _bus;
        private readonly Uri _destinationAddress;

        public ProfilePermissionManager(IBus bus) {
            _bus = bus;
            _destinationAddress = new Uri("queue:profile-permission-requests"); // @@TODO: Config.
        }

        public async Task GrantPermissionsTo(long userId, IEnumerable<ProfilePermissionDto> permissions) {
            var client = _bus.CreateRequestClient<GrantPermissions>(_destinationAddress);

            await client.GetResponse<GrantPermissionsSuccess>(new GrantPermissions {
                CorrelationId = Guid.NewGuid(),
                UserId = userId,
                Permissions = permissions.Select(p => new ProfilePermissionDtoMsg {
                    Scope = (int) p.Scope,
                    Flags = p.Flags
                })
            });
        }
    }
}
