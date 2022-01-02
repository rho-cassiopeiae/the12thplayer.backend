using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;

namespace Livescore.Application.Livescore.Discussion.Queries.GetDiscussionsForFixture {
    public class GetDiscussionsForFixtureQuery : IRequest<HandleResult<IEnumerable<DiscussionDto>>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
    }

    public class GetDiscussionsForFixtureQueryHandler : IRequestHandler<
        GetDiscussionsForFixtureQuery, HandleResult<IEnumerable<DiscussionDto>>
    > {
        private readonly IDiscussionInMemQueryable _discussionInMemQueryable;

        public GetDiscussionsForFixtureQueryHandler(
            IDiscussionInMemQueryable discussionInMemQueryable
        ) {
            _discussionInMemQueryable = discussionInMemQueryable;
        }

        public async Task<HandleResult<IEnumerable<DiscussionDto>>> Handle(
            GetDiscussionsForFixtureQuery query, CancellationToken cancellationToken
        ) {
            var discussions = await _discussionInMemQueryable.GetAllFor(query.FixtureId, query.TeamId);

            return new HandleResult<IEnumerable<DiscussionDto>> {
                Data = discussions.Select(d => new DiscussionDto {
                    Id = d.Id.ToString(),
                    Name = d.Name,
                    Active = d.Active
                })
            };
        }
    }
}
