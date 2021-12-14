using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Profile.Application.Common.Interfaces;
using Profile.Application.Common.Results;
using Profile.Domain.Aggregates.Profile;

namespace Profile.Application.Profile.Queries.GetPermissions {
    public class GetPermissionsQuery : IRequest<HandleResult<IEnumerable<ProfilePermission>>> {
        public long UserId { get; init; }
    }

    public class GetPermissionsQueryHandler : IRequestHandler<
        GetPermissionsQuery, HandleResult<IEnumerable<ProfilePermission>>
    > {
        private readonly IProfileQueryable _profileQueryable;

        public GetPermissionsQueryHandler(IProfileQueryable profileQueryable) {
            _profileQueryable = profileQueryable;
        }

        public async Task<HandleResult<IEnumerable<ProfilePermission>>> Handle(
            GetPermissionsQuery query, CancellationToken cancellationToken
        ) {
            return new HandleResult<IEnumerable<ProfilePermission>> {
                Data = await _profileQueryable.GetPermissionsFor(query.UserId)
            };
        }
    }
}
