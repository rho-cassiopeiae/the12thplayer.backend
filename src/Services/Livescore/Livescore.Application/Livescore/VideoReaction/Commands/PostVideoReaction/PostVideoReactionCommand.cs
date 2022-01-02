using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Application.Common.Attributes;
using Livescore.Application.Common.Interfaces;
using Livescore.Application.Livescore.Common.Errors;
using Livescore.Application.Common.Errors;
using Livescore.Domain.Base;
using Livescore.Application.Livescore.VideoReaction.Common.Errors;
using VideoReactionDm = Livescore.Domain.Aggregates.VideoReaction.VideoReaction;

namespace Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction {
    [RequireAuthorization]
    public class PostVideoReactionCommand : IRequest<HandleResult<string>> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
        public HttpRequest Request { get; init; }
    }

    public class PostVideoReactionCommandHandler : IRequestHandler<
        PostVideoReactionCommand, HandleResult<string>
    > {
        private readonly ILogger<PostVideoReactionCommandHandler> _logger;
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IFileReceiver _fileReceiver;
        private readonly IFileHosting _fileHosting;

        private readonly IInMemUnitOfWork _unitOfWork;
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IVideoReactionInMemRepository _videoReactionInMemRepository;

        private readonly IVideoReactionInMemQueryable _videoReactionInMemQueryable;

        public PostVideoReactionCommandHandler(
            ILogger<PostVideoReactionCommandHandler> logger,
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IFileReceiver fileReceiver,
            IFileHosting fileHosting,
            IInMemUnitOfWork unitOfWork,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IVideoReactionInMemRepository videoReactionInMemRepository,
            IVideoReactionInMemQueryable videoReactionInMemQueryable
        ) {
            _logger = logger;
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _fileReceiver = fileReceiver;
            _fileHosting = fileHosting;
            _unitOfWork = unitOfWork;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _videoReactionInMemRepository = videoReactionInMemRepository;
            _videoReactionInMemQueryable = videoReactionInMemQueryable;
        }

        public async Task<HandleResult<string>> Handle(
            PostVideoReactionCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);
            var username = _principalDataProvider.GetUsername(_authenticationContext.User);

            long postedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            bool active = await _fixtureLivescoreStatusInMemRepository.FindOutIfActive(
                command.FixtureId, command.TeamId
            );
            if (!active) {
                return new HandleResult<string> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            bool reserved = await _videoReactionInMemRepository.TryReserveSlotFor__immediate(
                command.FixtureId, command.TeamId, userId
            );
            if (!reserved) {
                return new HandleResult<string> {
                    Error = new VideoReactionError("Can only post one video reaction per fixture")
                };
            }

            FormCollection formValues = null;
            try {
                var outcome = await _fileReceiver.ReceiveVideoAndFormValues(
                    command.Request,
                    maxSize: 50 * 1024 * 1024, // @@TODO: Config.
                    filePrefix: $"video-reactions/f-{command.FixtureId}-t-{command.TeamId}"
                );
                if (outcome.IsError) {
                    return new HandleResult<string> {
                        Error = outcome.Error
                    };
                }
                if (!outcome.Data.ContainsKey("title")) { // @@TODO: Validate title.
                    _fileReceiver.DeleteFile(outcome.Data["filePath"]);

                    return new HandleResult<string> {
                        Error = new ValidationError("No title provided")
                    };
                }

                formValues = outcome.Data;
            } catch (Exception e) {
                _logger.LogError(e, "Error receiving user video");

                return new HandleResult<string> {
                    Error = new VideoReactionError("Error uploading video reaction")
                };
            } finally {
                if (formValues == null) {
                    await _videoReactionInMemRepository.ReleaseSlotFor__immediate(
                        command.FixtureId, command.TeamId, userId
                    );
                }
            }

            var filePath = formValues["filePath"];

            var vimeoProjectId = await _videoReactionInMemQueryable.GetVimeoProjectIdFor(
                command.FixtureId, command.TeamId
            );
            if (vimeoProjectId == null) {
                // @@IRRELEVANT
                // @@NOTE: Means that in between checking if fixture is still active and retrieving its vimeo
                // project id, it has been finalized (fully).

                // @@IRRELEVANT
                // @@TODO??: There is a very small chance that the cleanup happened right in between the active
                // status check and slot reservation. So by reserving a slot we affectively reintroduced the
                // 'f:xxx.t:yyy.video-reaction-author-ids' set. Should nuke it here instead of just releasing?
                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Error deleting user video");
                }

                await _videoReactionInMemRepository.ReleaseSlotFor__immediate(command.FixtureId, command.TeamId, userId);

                return new HandleResult<string> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            string videoId;
            try {
                videoId = await _fileHosting.UploadVideo(filePath, vimeoProjectId);
            } catch (Exception e) {
                _logger.LogError(e, "Error uploading user video");

                // @@NOTE: Since here it's unclear what exactly caused uploading to fail (it could be just a MassTransit
                // error for all we know), we should not rely on FileHostingGateway deleting the file. Need to ensure that
                // it gets properly deleted.

                // One of the possible errors (6) is that the project no longer existed when FileHostingGateway tried to put
                // the uploaded video into it.

                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Error deleting user video");
                }

                await _videoReactionInMemRepository.ReleaseSlotFor__immediate(command.FixtureId, command.TeamId, userId);

                return new HandleResult<string> {
                    Error = e.Message.Contains("(6)") ?
                        new LivescoreError("Fixture is no longer active") :
                        new VideoReactionError("Error uploading video reaction")
                };
            }

            _unitOfWork.Begin();

            _fixtureLivescoreStatusInMemRepository.EnlistAsPartOf(_unitOfWork);
            _fixtureLivescoreStatusInMemRepository.WatchStillActive(command.FixtureId, command.TeamId);

            var videoReaction = new VideoReactionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                authorId: userId,
                authorUsername: username,
                title: formValues["title"],
                videoId: videoId,
                postedAt: postedAt,
                rating: 1
            );

            _videoReactionInMemRepository.EnlistAsPartOf(_unitOfWork);
            _videoReactionInMemRepository.Create(videoReaction);

            bool applied = await _unitOfWork.Commit();
            if (!applied) { // @@NOTE: Means the fixture is no longer active.
                // If here, the video [has already been/will be] deleted from vimeo together with the project (since
                // successful upload means the project still existed at that time).
                
                await _videoReactionInMemRepository.ReleaseSlotFor__immediate(command.FixtureId, command.TeamId, userId);

                return new HandleResult<string> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            return new HandleResult<string> {
                Data = videoId
            };
        }
    }
}
