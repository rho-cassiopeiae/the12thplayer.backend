using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using MediatR;

using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.VideoReaction;
using Livescore.Domain.Aggregates.FixtureLivescoreStatus;
using VideoReactionDm = Livescore.Domain.Aggregates.VideoReaction.VideoReaction;

namespace Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction {
    public class PostVideoReactionCommand : IRequest<HandleResult<VideoReactionStreamingInfoDto>> {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public HttpRequest Request { get; set; }
    }

    public class PostVideoReactionCommandHandler : IRequestHandler<
        PostVideoReactionCommand, HandleResult<VideoReactionStreamingInfoDto>
    > {
        private readonly IFixtureLivescoreStatusInMemRepository _fixtureLivescoreStatusInMemRepository;
        private readonly IVideoReactionInMemRepository _videoReactionInMemRepository;

        public PostVideoReactionCommandHandler(
            IFixtureLivescoreStatusInMemRepository fixtureLivescoreStatusInMemRepository,
            IVideoReactionInMemRepository videoReactionInMemRepository
        ) {
            _fixtureLivescoreStatusInMemRepository = fixtureLivescoreStatusInMemRepository;
            _videoReactionInMemRepository = videoReactionInMemRepository;
        }

        public async Task<HandleResult<VideoReactionStreamingInfoDto>> Handle(
            PostVideoReactionCommand command, CancellationToken cancellationToken
        ) {
            long userId = 1;

            var videoReaction = new VideoReactionDm(
                fixtureId: command.FixtureId,
                teamId: command.TeamId,
                authorId: userId,
                authorUsername: "rho9cas",
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
