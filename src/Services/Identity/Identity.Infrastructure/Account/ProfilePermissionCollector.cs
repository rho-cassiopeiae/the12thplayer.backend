using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Identity.Application.Account.Commands.LogInAsAdmin;
using Identity.Application.Common.Interfaces;

using MassTransit;

using MessageBus.Contracts.Requests.Identity;
using MessageBus.Contracts.Responses.Profile;

namespace Identity.Infrastructure.Account {
    public class ProfilePermissionCollector : IProfilePermissionCollector {
        private readonly IRequestClient<CollectProfilePermissions>
            _collectPermissionsClient;

        public ProfilePermissionCollector(
            IRequestClient<CollectProfilePermissions> collectPermissionsClient
        ) {
            _collectPermissionsClient = collectPermissionsClient;
        }

        public async Task<IEnumerable<ProfilePermission>> CollectPermissionsFor(
            long userId
        ) {
            var response = await _collectPermissionsClient.GetResponse<
                CollectProfilePermissionsSuccess
            >(new CollectProfilePermissions {
                CorrelationId = Guid.NewGuid(),
                UserId = userId
            });

            return response.Message.Permissions.Select(
                p => new ProfilePermission {
                    Scope = (PermissionScope) p.Scope,
                    Flags = p.Flags
                }
            );
        }
    }
}
