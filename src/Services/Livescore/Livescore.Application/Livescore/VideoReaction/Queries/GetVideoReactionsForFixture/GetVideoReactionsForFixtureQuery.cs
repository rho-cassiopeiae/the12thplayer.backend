﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Common.Errors;

namespace Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture {
    public class GetVideoReactionsForFixtureQuery : IRequest<
        HandleResult<IEnumerable<VideoReactionWithUserVoteDto>>
    > {
        public long FixtureId { get; set; }
        public long TeamId { get; set; }
        public int Filter { get; set; }
        public int Start { get; set; }
    }

    public class GetVideoReactionsForFixtureQueryHandler : IRequestHandler<
        GetVideoReactionsForFixtureQuery,
        HandleResult<IEnumerable<VideoReactionWithUserVoteDto>>
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IFixtureLivescoreStatusInMemQueryable _fixtureLivescoreStatusInMemQueryable;
        private readonly IVideoReactionInMemQueryable _videoReactionInMemQueryable;
        private readonly IUserVoteInMemQueryable _userVoteInMemQueryable;

        public GetVideoReactionsForFixtureQueryHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IFixtureLivescoreStatusInMemQueryable fixtureLivescoreStatusInMemQueryable,
            IVideoReactionInMemQueryable videoReactionInMemQueryable,
            IUserVoteInMemQueryable userVoteInMemQueryable
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _fixtureLivescoreStatusInMemQueryable = fixtureLivescoreStatusInMemQueryable;
            _videoReactionInMemQueryable = videoReactionInMemQueryable;
            _userVoteInMemQueryable = userVoteInMemQueryable;
        }

        public async Task<HandleResult<IEnumerable<VideoReactionWithUserVoteDto>>> Handle(
            GetVideoReactionsForFixtureQuery query, CancellationToken cancellationToken
        ) {
            bool active = await _fixtureLivescoreStatusInMemQueryable.CheckActive(
                query.FixtureId, query.TeamId
            );
            if (!active) {
                return new HandleResult<IEnumerable<VideoReactionWithUserVoteDto>> {
                    Error = new LivescoreError("Fixture is not active")
                };
            }

            var videoReactions = await _videoReactionInMemQueryable.GetAllFor(
                query.FixtureId, query.TeamId, (VideoReactionFilter) query.Filter, query.Start
            );

            var videoReactionsWithUserVote = videoReactions
                .Select(vr => new VideoReactionWithUserVoteDto {
                    AuthorId = vr.AuthorId,
                    Title = vr.Title,
                    AuthorUsername = vr.AuthorUsername,
                    Rating = vr.Rating,
                    VideoId = vr.VideoId,
                    ThumbnailUrl = vr.ThumbnailUrl
                })
                .ToList();

            if (videoReactionsWithUserVote.Any() && _authenticationContext.User != null) {
                long userId = _principalDataProvider.GetId(_authenticationContext.User);

                var userVote = await _userVoteInMemQueryable.GetVideoReactionVotesFor(
                    userId, query.FixtureId, query.TeamId,
                    videoReactionsWithUserVote.Select(vr => vr.AuthorId).ToList()
                );
                if (userVote != null) {
                    foreach (var authorIdToVote in userVote.VideoReactionAuthorIdToVote) {
                        var authorId = long.Parse(authorIdToVote.Key);
                        var videoReactionWithUserVote = videoReactionsWithUserVote.First(vr => vr.AuthorId == authorId);
                        videoReactionWithUserVote.UserVote = authorIdToVote.Value.Value;
                    }
                }
            }

            return new HandleResult<IEnumerable<VideoReactionWithUserVoteDto>> {
                Data = videoReactionsWithUserVote
            };
        }
    }
}
