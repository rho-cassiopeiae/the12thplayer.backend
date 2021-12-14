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
    public class ProfileSvcQueryable : IProfileSvcQueryable {
        private readonly IRequestClient<GetProfilePermissions> _getPermissionsClient;

        public ProfileSvcQueryable(
            IRequestClient<GetProfilePermissions> getPermissionsClient
        ) {
            _getPermissionsClient = getPermissionsClient;
        }

        public async Task<IEnumerable<ProfilePermissionDto>> GetPermissionsFor(long userId) {
            var response = await _getPermissionsClient.GetResponse<GetProfilePermissionsSuccess>(
                new GetProfilePermissions {
                    CorrelationId = Guid.NewGuid(),
                    UserId = userId
                }
            );

            return response.Message.Permissions.Select(
                p => new ProfilePermissionDto {
                    Scope = (PermissionScope) p.Scope,
                    Flags = p.Flags
                }
            );
        }
    }
}
