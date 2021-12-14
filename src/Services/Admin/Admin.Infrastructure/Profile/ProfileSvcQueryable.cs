using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Responses.Profile;
using MessageBus.Contracts.Common.Dto;

using Admin.Application.Common.Attributes;
using Admin.Application.Common.Interfaces;

namespace Admin.Infrastructure.Profile {
    public class ProfileSvcQueryable : IProfileSvcQueryable {
        private readonly IRequestClient<CheckProfileHasPermissions> _checkPermissionsClient;

        public ProfileSvcQueryable(
            IRequestClient<CheckProfileHasPermissions> checkPermissionsClient
        ) {
            _checkPermissionsClient = checkPermissionsClient;
        }

        public async Task<bool> CheckHasPermissions(
            long userId,
            IEnumerable<RequirePermissionAttribute> permissionAttributes
        ) {
            var response = await _checkPermissionsClient.GetResponse<CheckProfileHasPermissionsSuccess>(
                new CheckProfileHasPermissions {
                    CorrelationId = Guid.NewGuid(),
                    UserId = userId,
                    Permissions = permissionAttributes.Select(pa =>
                        new ProfilePermissionDto {
                            Scope = (int) pa.Scope,
                            Flags = pa.Flags
                        }
                    )
                }
            );

            return response.Message.HasRequiredPermissions;
        }
    }
}
