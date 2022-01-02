using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Common.Errors;

namespace Livescore.Application.Livescore.Discussion.Commands.SubscribeToDiscussion {
    public class SubscribeToDiscussionCommand : IRequest<HandleResult<FixtureDiscussionUpdateDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string DiscussionId { get; set; }
    }

    public class SubscribeToDiscussionCommandHandler : IRequestHandler<
        SubscribeToDiscussionCommand, HandleResult<FixtureDiscussionUpdateDto>
    > {
        private readonly IConnectionIdProvider _connectionIdProvider;
        private readonly IFixtureLivescoreStatusInMemQueryable _fixtureLivescoreStatusInMemQueryable;
        private readonly IDiscussionInMemQueryable _discussionInMemQueryable;
        private readonly IFixtureDiscussionBroadcaster _fixtureDiscussionBroadcaster;

        public SubscribeToDiscussionCommandHandler(
            IConnectionIdProvider connectionIdProvider,
            IFixtureLivescoreStatusInMemQueryable fixtureLivescoreStatusInMemQueryable,
            IDiscussionInMemQueryable discussionInMemQueryable,
            IFixtureDiscussionBroadcaster fixtureDiscussionBroadcaster
        ) {
            _connectionIdProvider = connectionIdProvider;
            _fixtureLivescoreStatusInMemQueryable = fixtureLivescoreStatusInMemQueryable;
            _discussionInMemQueryable = discussionInMemQueryable;
            _fixtureDiscussionBroadcaster = fixtureDiscussionBroadcaster;
        }

        public async Task<HandleResult<FixtureDiscussionUpdateDto>> Handle(
            SubscribeToDiscussionCommand command, CancellationToken cancellationToken
        ) {
            bool active = await _fixtureLivescoreStatusInMemQueryable.CheckActive(
                command.FixtureId, command.TeamId
            );
            if (!active) {
                return new HandleResult<FixtureDiscussionUpdateDto> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            string connectionId = _connectionIdProvider.ConnectionId;

            await _fixtureDiscussionBroadcaster.SubscribeToDiscussion(
                connectionId, command.FixtureId, command.TeamId, command.DiscussionId
            );

            // @@NOTE: First subscribe and *then* retrieve entries, because if do it the other way around
            // there is a small chance that we miss an entry posted in between two calls.

            var entries = await _discussionInMemQueryable.GetEntriesFor(
                command.FixtureId, command.TeamId, Guid.Parse(command.DiscussionId)
            );
            //if (!entries.Any()) {
            //    // @@IRRELEVANT: Since we do a delayed clean up now, this case is no longer relevant.
            //    // @@NOTE: An unlikely case when in between checking the fixture's active status
            //    // and retrieving the entries, it is finalized and the discussion is cleaned up. There is
            //    // always at least one entry in a discussion, which means if there are none, it's been cleaned up.
            //    await _fixtureDiscussionBroadcaster.UnsubscribeFromDiscussion(
            //        connectionId, command.FixtureId, command.TeamId, command.DiscussionId
            //    );
            //}

            return new HandleResult<FixtureDiscussionUpdateDto> {
                Data = new FixtureDiscussionUpdateDto {
                    FixtureId = command.FixtureId,
                    TeamId = command.TeamId,
                    DiscussionId = command.DiscussionId,
                    Entries = entries.Select(e => new DiscussionEntryDto {
                        Id = e.Id,
                        UserId = e.UserId,
                        Username = e.Username,
                        Body = e.Body
                    })
                }
            };
        }
    }
}
