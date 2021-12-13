using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Domain.Aggregates.Discussion;
using Livescore.Domain.Base;
using Livescore.Domain.Aggregates.VideoReaction;
using DiscussionDm = Livescore.Domain.Aggregates.Discussion.Discussion;

namespace Livescore.Application.Livescore.Worker.Commands.ActivateFixture {
    public class ActivateFixtureCommand : IRequest<VoidResult> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public string VimeoProjectId { get; init; }
    }

    public class ActivateFixtureCommandHandler : IRequestHandler<
        ActivateFixtureCommand, VoidResult
    > {
        private readonly IInMemUnitOfWork _unitOfWork;
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IDiscussionInMemRepository _discussionInMemRepository;
        private readonly IVideoReactionInMemRepository _videoReactionInMemRepository;

        public ActivateFixtureCommandHandler(
            IInMemUnitOfWork unitOfWork,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IDiscussionInMemRepository discussionInMemRepository,
            IVideoReactionInMemRepository videoReactionInMemRepository
        ) {
            _unitOfWork = unitOfWork;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _discussionInMemRepository = discussionInMemRepository;
            _videoReactionInMemRepository = videoReactionInMemRepository;
        }

        public async Task<VoidResult> Handle(
            ActivateFixtureCommand command, CancellationToken cancellationToken
        ) {
            _unitOfWork.Begin();

            _fixtureLivescoreStatusInMemRepository.EnlistAsPartOf(_unitOfWork);

            var fixtureStatus = new FixtureLivescoreStatus(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                active: true,
                ongoing: true
            );

            _fixtureLivescoreStatusInMemRepository.SetOrUpdate(fixtureStatus);

            _discussionInMemRepository.EnlistAsPartOf(_unitOfWork);

            var prematchDiscussion = new DiscussionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                id: Guid.NewGuid(),
                name: "pre-match",
                active: true
            );
            prematchDiscussion.AddEntry(new DiscussionEntry(
                id: "0-1",
                userId: 1,
                username: "The12thPlayer",
                body:
                    $"Welcome to the {prematchDiscussion.Name} discussion room. Please keep it civil. " +
                    "Calling players useless dickheads and swearing at the referee is fine though :) " +
                    "You can post once every 30 seconds. Thank you."
            ));

            var matchDiscussion = new DiscussionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                id: Guid.NewGuid(),
                name: "match",
                active: true
            );
            matchDiscussion.AddEntry(new DiscussionEntry(
                id: "0-1",
                userId: 1,
                username: "The12thPlayer",
                body:
                    $"Welcome to the {matchDiscussion.Name} discussion room. Please keep it civil. " +
                    "Calling players useless dickheads and swearing at the referee is fine though :) " +
                    "You can post once every 30 seconds. Thank you."
            ));

            var postmatchDiscussion = new DiscussionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                id: Guid.NewGuid(),
                name: "post-match",
                active: true
            );
            postmatchDiscussion.AddEntry(new DiscussionEntry(
                id: "0-1",
                userId: 1,
                username: "The12thPlayer",
                body:
                    $"Welcome to the {postmatchDiscussion.Name} discussion room. Please keep it civil. " +
                    "Calling players useless dickheads and swearing at the referee is fine though :) " +
                    "You can post once every 30 seconds. Thank you."
            ));

            _discussionInMemRepository.Create(prematchDiscussion);
            _discussionInMemRepository.Create(matchDiscussion);
            _discussionInMemRepository.Create(postmatchDiscussion);

            _videoReactionInMemRepository.EnlistAsPartOf(_unitOfWork);
            _videoReactionInMemRepository.SetVimeoProjectIdFor(
                command.FixtureId, command.TeamId, command.VimeoProjectId
            );

            await _unitOfWork.Commit();

            return VoidResult.Instance;
        }
    }
}
