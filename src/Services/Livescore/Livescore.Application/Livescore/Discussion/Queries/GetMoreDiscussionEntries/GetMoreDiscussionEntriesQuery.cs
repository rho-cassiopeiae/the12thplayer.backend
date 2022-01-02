using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Common.Dto;
using Livescore.Application.Livescore.Common.Errors;

namespace Livescore.Application.Livescore.Discussion.Queries.GetMoreDiscussionEntries {
    public class GetMoreDiscussionEntriesQuery : IRequest<HandleResult<IEnumerable<DiscussionEntryDto>>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string DiscussionId { get; set; }
        public string LastReceivedEntryId { get; set; }
    }

    public class GetMoreDiscussionEntriesQueryHandler : IRequestHandler<
        GetMoreDiscussionEntriesQuery, HandleResult<IEnumerable<DiscussionEntryDto>>
    > {
        private readonly IFixtureLivescoreStatusInMemQueryable _fixtureLivescoreStatusInMemQueryable;
        private readonly IDiscussionInMemQueryable _discussionInMemQueryable;

        public GetMoreDiscussionEntriesQueryHandler(
            IFixtureLivescoreStatusInMemQueryable fixtureLivescoreStatusInMemQueryable,
            IDiscussionInMemQueryable discussionInMemQueryable
        ) {
            _fixtureLivescoreStatusInMemQueryable = fixtureLivescoreStatusInMemQueryable;
            _discussionInMemQueryable = discussionInMemQueryable;
        }

        public async Task<HandleResult<IEnumerable<DiscussionEntryDto>>> Handle(
            GetMoreDiscussionEntriesQuery query, CancellationToken cancellationToken
        ) {
            bool active = await _fixtureLivescoreStatusInMemQueryable.CheckActive(
                query.FixtureId, query.TeamId
            );
            if (!active) {
                return new HandleResult<IEnumerable<DiscussionEntryDto>> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            var entries = await _discussionInMemQueryable.GetEntriesFor(
                query.FixtureId, query.TeamId, Guid.Parse(query.DiscussionId), query.LastReceivedEntryId
            );

            return new HandleResult<IEnumerable<DiscussionEntryDto>> {
                Data = entries.Select(e => new DiscussionEntryDto {
                    Id = e.Id,
                    UserId = e.UserId,
                    Username = e.Username,
                    Body = e.Body
                })
            };
        }
    }
}
