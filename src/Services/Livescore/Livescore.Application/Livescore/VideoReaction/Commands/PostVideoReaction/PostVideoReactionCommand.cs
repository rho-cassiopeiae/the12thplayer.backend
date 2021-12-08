using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using Livescore.Application.Common.Attributes;
using Livescore.Application.Common.Interfaces;
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
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IVideoReactionInMemRepository _videoReactionInMemRepository;

        public PostVideoReactionCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IVideoReactionInMemRepository videoReactionInMemRepository
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _videoReactionInMemRepository = videoReactionInMemRepository;
        }

        public async Task<HandleResult<VideoReactionStreamingInfoDto>> Handle(
            PostVideoReactionCommand command, CancellationToken cancellationToken
        ) {
            long userId = _principalDataProvider.GetId(_authenticationContext.User);
            var username = _principalDataProvider.GetUsername(_authenticationContext.User);

            var videoReaction = new VideoReactionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                authorId: userId,
                authorUsername: username,
                title: "title",
                videoId: "videoId",
                thumbnailUrl: "thumbnailUrl",
                postedAt: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                rating: 1
            );

            _videoReactionInMemRepository.Create(videoReaction);

            await _videoReactionInMemRepository.SaveChanges();

            return new HandleResult<VideoReactionStreamingInfoDto> {
                Data = new VideoReactionStreamingInfoDto {
                    VideoId = videoReaction.VideoId,
                    ThumbnailUrl = videoReaction.ThumbnailUrl
                }
            };
        }
    }
}
