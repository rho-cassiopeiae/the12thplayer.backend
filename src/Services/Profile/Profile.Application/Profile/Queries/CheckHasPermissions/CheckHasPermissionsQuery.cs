using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Profile.Application.Common.Interfaces;
using Profile.Application.Common.Results;
using Profile.Application.Profile.Common.Dto;

namespace Profile.Application.Profile.Queries.CheckHasPermissions {
    public class CheckHasPermissionsQuery : IRequest<HandleResult<bool>> {
        public long UserId { get; init; }
        public IEnumerable<ProfilePermissionDto> Permissions { get; init; }
    }

    public class CheckHasPermissionsQueryHandler : IRequestHandler<
        CheckHasPermissionsQuery, HandleResult<bool>
    > {
        private readonly IProfileQueryable _profileQueryable;

        public CheckHasPermissionsQueryHandler(IProfileQueryable profileQueryable) {
            _profileQueryable = profileQueryable;
        }

        public async Task<HandleResult<bool>> Handle(
            CheckHasPermissionsQuery query, CancellationToken cancellationToken
        ) {
            var permissions = await _profileQueryable.GetPermissionsFor(query.UserId);

            foreach (var requiredPermission in query.Permissions) {
                if (
                    !permissions.Any(p =>
                        (int) p.Scope == requiredPermission.Scope &&
                        (p.Flags & requiredPermission.Flags) == requiredPermission.Flags
                    )
                ) {
                    return new HandleResult<bool> {
                        Data = false
                    };
                }
            }

            return new HandleResult<bool> {
                Data = true
            };
        }
    }
}
