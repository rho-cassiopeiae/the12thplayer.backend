using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Attributes;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Common.Errors;
using Livescore.Application.Livescore.Discussion.Common.Errors;
using Livescore.Domain.Aggregates.Discussion;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Domain.Base;
using DiscussionDm = Livescore.Domain.Aggregates.Discussion.Discussion;

namespace Livescore.Application.Livescore.Discussion.Commands.PostDiscussionEntry {
    [RequireAuthorization]
    public class PostDiscussionEntryCommand : IRequest<VoidResult> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public string DiscussionId { get; set; }
        public string Body { get; set; }
    }

    public class PostDiscussionEntryCommandHandler : IRequestHandler<PostDiscussionEntryCommand, VoidResult> {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IInMemUnitOfWork _unitOfWork;
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IDiscussionInMemRepository _discussionInMemRepository;

        public PostDiscussionEntryCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IInMemUnitOfWork unitOfWork,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IDiscussionInMemRepository discussionInMemRepository
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _unitOfWork = unitOfWork;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _discussionInMemRepository = discussionInMemRepository;
        }

        public async Task<VoidResult> Handle(
            PostDiscussionEntryCommand command, CancellationToken cancellationToken
        ) {
            bool active = await _fixtureLivescoreStatusInMemRepository.FindOutIfActive(
                command.FixtureId, command.TeamId
            );
            if (!active) {
                return new VoidResult {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            _unitOfWork.Begin();

            _fixtureLivescoreStatusInMemRepository.EnlistAsPartOf(_unitOfWork);
            _fixtureLivescoreStatusInMemRepository.WatchStillActive(command.FixtureId, command.TeamId);

            _discussionInMemRepository.EnlistAsPartOf(_unitOfWork);

            var discussion = new DiscussionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                id: Guid.Parse(command.DiscussionId)
            );

            _discussionInMemRepository.WatchStillActive(discussion);

            discussion.AddEntry(new DiscussionEntry(
                id: "*",
                userId: _principalDataProvider.GetId(_authenticationContext.User),
                username: _principalDataProvider.GetUsername(_authenticationContext.User),
                body: command.Body
            ));

            _discussionInMemRepository.PostEntries(discussion);

            bool applied = await _unitOfWork.Commit();
            if (!applied) {
                // @@NOTE: Either the fixture is no longer active, or the discussion is no longer active (or it doesn't exist at all),
                // or it's a rate-limit error.

                // @@TODO: Distinct error message for rate-limit error.
                return new VoidResult {
                    Error = new DiscussionError("Error trying to post a new discussion entry")
                };
            }

            return VoidResult.Instance;
        }
    }
}
