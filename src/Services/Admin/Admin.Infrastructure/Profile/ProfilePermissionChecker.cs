using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Responses.Profile;

using Admin.Application.Common.Attributes;
using Admin.Application.Common.Interfaces;
using System;

namespace Admin.Infrastructure.Profile {
    public class ProfilePermissionChecker : IProfilePermissionChecker {
        private readonly IRequestClient<CheckProfileHasPermissions>
            _checkPermissionsClient;

        public ProfilePermissionChecker(
            IRequestClient<CheckProfileHasPermissions> checkPermissionsClient
        ) {
            _checkPermissionsClient = checkPermissionsClient;
        }

        public async Task<bool> HasPermissions(
            long userId,
            IEnumerable<RequirePermissionAttribute> permissionAttributes
        ) {
            var response = await _checkPermissionsClient.GetResponse<
                CheckProfileHasPermissionsSuccess
            >(new CheckProfileHasPermissions {
                CorrelationId = Guid.NewGuid(),
                UserId = userId,
                Permissions = permissionAttributes.Select(pa =>
                    new CheckProfileHasPermissions.ProfilePermission {
                        Scope = (int) pa.Scope,
                        Flags = pa.Flags
                    }
                )
            });

            return response.Message.HasRequiredPermissions;
        }
    }
}
