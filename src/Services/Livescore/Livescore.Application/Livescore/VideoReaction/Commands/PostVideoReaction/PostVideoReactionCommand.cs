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
using Livescore.Domain.Base;
using Livescore.Application.Livescore.VideoReaction.Common.Errors;
using VideoReactionDm = Livescore.Domain.Aggregates.VideoReaction.VideoReaction;

namespace Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction {
    [RequireAuthorization]
    public class PostVideoReactionCommand : IRequest<HandleResult<VideoReactionStreamingInfoDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public HttpRequest Request { get; set; }
    }

    public class PostVideoReactionCommandHandler : IRequestHandler<
        PostVideoReactionCommand, HandleResult<VideoReactionStreamingInfoDto>
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

        public async Task<HandleResult<VideoReactionStreamingInfoDto>> Handle(
            PostVideoReactionCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);
            var username = _principalDataProvider.GetUsername(_authenticationContext.User);

            bool active = await _fixtureLivescoreStatusInMemRepository.FindOutIfActive(
                command.FixtureId, command.TeamId
            );
            if (!active) {
                return new HandleResult<VideoReactionStreamingInfoDto> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            bool reserved = await _videoReactionInMemRepository.TryReserveSlotFor__immediate(
                command.FixtureId, command.TeamId, userId
            );
            if (!reserved) {
                return new HandleResult<VideoReactionStreamingInfoDto> {
                    Error = new VideoReactionError("Can only post one video reaction per fixture")
                };
            }

            FormCollection formValues = null;
            try {
                var result = await _fileReceiver.ReceiveVideoAndFormValues(
                    command.Request,
                    maxSize: 50 * 1024 * 1024, // @@TODO: Config.
                    filePrefix: $"video-reactions/f-{command.FixtureId}-t-{command.TeamId}"
                );
                if (result.IsError) {
                    return new HandleResult<VideoReactionStreamingInfoDto> {
                        Error = result.Error
                    };
                }
                if (!result.Data.ContainsKey("title")) { // @@TODO: Validate title.
                    _fileReceiver.DeleteFile(result.Data["filePath"]);

                    return new HandleResult<VideoReactionStreamingInfoDto> {
                        Error = new VideoReactionError("No title provided")
                    };
                }

                formValues = result.Data;
            } catch (Exception e) {
                _logger.LogError("Error receiving user video", e);

                return new HandleResult<VideoReactionStreamingInfoDto> {
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
                // @@NOTE: Means that in between checking if fixture is still active and retrieving its vimeo
                // project id, it has been finalized (fully).

                // @@TODO??: There is a very small chance that the cleanup happened right in between the active
                // status check and slot reservation. So by reserving a slot we affectively reintroduced the
                // 'f:xxx.t:yyy.video-reaction-author-ids' set. Should nuke it here instead of just releasing?
                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception ex) {
                    _logger.LogError("Error deleting user video", ex);
                }

                await _videoReactionInMemRepository.ReleaseSlotFor__immediate(command.FixtureId, command.TeamId, userId);

                return new HandleResult<VideoReactionStreamingInfoDto> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            // @@NOTE: Means that even if the fixture is getting finalized at this very moment,
            // the cleanup hasn't happened yet.

            string videoId, thumbnailUrl;
            try {
                (videoId, thumbnailUrl) = await _fileHosting.UploadVideo(filePath, vimeoProjectId);
            } catch (Exception e) {
                _logger.LogError("Error uploading user video", e);

                // @@TODO: Since here it's unclear what exactly caused uploading to fail (it could be just a MassTransit
                // error for all we know), we should not rely on FileHostingGateway deleting the file. Need to ensure that
                // it gets properly deleted.

                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception ex) {
                    _logger.LogError("Error deleting user video", ex);
                }

                await _videoReactionInMemRepository.ReleaseSlotFor__immediate(command.FixtureId, command.TeamId, userId);

                return new HandleResult<VideoReactionStreamingInfoDto> {
                    Error = new VideoReactionError("Error uploading video reaction")
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
                thumbnailUrl: thumbnailUrl,
                postedAt: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                rating: 1
            );

            _videoReactionInMemRepository.EnlistAsPartOf(_unitOfWork);
            _videoReactionInMemRepository.Create(videoReaction);

            bool applied = await _unitOfWork.Commit();
            if (!applied) { // @@NOTE: Means the fixture is no longer active.
                // @@TODO: Should manually request file deletion from vimeo, since at this point it's unclear whether
                // the file was uploaded and /then/ the entire vimeo folder was deleted, or the other way around.

                // @@NOTE: Don't need to delete the file from the file system. Successful upload means it has already been deleted.
                
                await _videoReactionInMemRepository.ReleaseSlotFor__immediate(command.FixtureId, command.TeamId, userId);

                return new HandleResult<VideoReactionStreamingInfoDto> {
                    Error = new LivescoreError("Fixture is no longer active")
                };
            }

            return new HandleResult<VideoReactionStreamingInfoDto> {
                Data = new VideoReactionStreamingInfoDto {
                    VideoId = videoId,
                    ThumbnailUrl = thumbnailUrl
                }
            };
        }
    }
}
