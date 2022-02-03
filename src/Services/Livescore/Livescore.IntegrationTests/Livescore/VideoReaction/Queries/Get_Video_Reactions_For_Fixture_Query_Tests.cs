using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.VideoReaction.Commands.PostVideoReaction;
using Livescore.Application.Livescore.VideoReaction.Commands.VoteForVideoReaction;
using Livescore.Application.Livescore.VideoReaction.Queries.GetVideoReactionsForFixture;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.IntegrationTests.Livescore.VideoReaction.Mocks;

namespace Livescore.IntegrationTests.Livescore.VideoReaction.Queries {
    [Collection(nameof(LivescoreTestCollection))]
    public class Get_Video_Reactions_For_Fixture_Query_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly List<long> _authorIds = new();
        private readonly List<string> _authorUsernames = new();

        public Get_Video_Reactions_For_Fixture_Query_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            (_fixtureId, _teamId) = _sut.SeedWithDummyUpcomingFixture();

            _sut.SendRequest(
                new ActivateFixtureCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    VimeoProjectId = "789456"
                }
            ).Wait();

            var tasks = new List<Task>();
            for (int i = 1; i < 7; ++i) {
                Func<int, Task> f = async userId => {
                    using var preparedRequest = _sut.PrepareHttpRequestForFileUpload(
                        "test-video.mp4",
                        new KeyValuePair<string, string>("title", $"test-title-{userId}")
                    );

                    var authorId = userId;
                    var authorUsername = $"user-{userId}";
                    _authorIds.Add(authorId);
                    _authorUsernames.Add(authorUsername);

                    _sut.RunAs(userId: authorId, username: authorUsername);

                    await _sut.SendRequest(
                        new PostVideoReactionCommand {
                            FixtureId = _fixtureId,
                            TeamId = _teamId,
                            Request = preparedRequest.Request
                        }
                    );
                };

                tasks.Add(f(i));
            }

            Task.WhenAll(tasks).Wait();
        }

        [Fact]
        public async Task Should_Retrieve_Top_Video_Reactions_With_User_Votes() {
            _sut.RunAs(userId: 100, username: "user-100");

            await Task.WhenAll(new[] {
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorIds[1],
                    UserVote = 1
                }),
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorIds[2],
                    UserVote = -1
                })
            });

            var result = await _sut.SendRequest(new GetVideoReactionsForFixtureQuery {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                Filter = VideoReactionFilter.Top,
                Page = 1
            });

            result.Data.Page.Should().Be(1);
            result.Data.TotalPages.Should().Be(1);

            var videoReactions = result.Data.VideoReactions;

            videoReactions.Should().HaveCount(6);

            videoReactions.First().Should().BeEquivalentTo(new VideoReactionWithUserVoteDto {
                AuthorId = _authorIds[1],
                Title = $"test-title-{_authorIds[1]}",
                AuthorUsername = _authorUsernames[1],
                Rating = 2,
                VideoId = FileHostingMock.VideoId,
                UserVote = 1
            });

            videoReactions.Last().Should().BeEquivalentTo(new VideoReactionWithUserVoteDto {
                AuthorId = _authorIds[2],
                Title = $"test-title-{_authorIds[2]}",
                AuthorUsername = _authorUsernames[2],
                Rating = 0,
                VideoId = FileHostingMock.VideoId,
                UserVote = -1
            });

            videoReactions.First(vr => vr.AuthorId == _authorIds[0]).Should().BeEquivalentTo(
                new VideoReactionWithUserVoteDto {
                    AuthorId = _authorIds[0],
                    Title = $"test-title-{_authorIds[0]}",
                    AuthorUsername = _authorUsernames[0],
                    Rating = 1,
                    VideoId = FileHostingMock.VideoId,
                    UserVote = null
                }
            );
        }

        [Fact]
        public async Task Should_Retrieve_Newest_Video_Reactions_With_User_Votes() {
            _sut.RunAs(userId: 100, username: "user-100");

            await Task.WhenAll(new[] {
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorIds[5],
                    UserVote = 1
                }),
                _sut.SendRequest(new VoteForVideoReactionCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    AuthorId = _authorIds[3],
                    UserVote = -1
                })
            });

            var result = await _sut.SendRequest(new GetVideoReactionsForFixtureQuery {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                Filter = VideoReactionFilter.Newest,
                Page = 1
            });

            result.Data.Page.Should().Be(1);
            result.Data.TotalPages.Should().Be(1);

            var videoReactions = result.Data.VideoReactions;

            videoReactions.Should().HaveCount(6);

            videoReactions.First().Should().BeEquivalentTo(new VideoReactionWithUserVoteDto {
                AuthorId = _authorIds[5],
                Title = $"test-title-{_authorIds[5]}",
                AuthorUsername = _authorUsernames[5],
                Rating = 2,
                VideoId = FileHostingMock.VideoId,
                UserVote = 1
            });

            videoReactions.Skip(1).First().Should().BeEquivalentTo(new VideoReactionWithUserVoteDto {
                AuthorId = _authorIds[4],
                Title = $"test-title-{_authorIds[4]}",
                AuthorUsername = _authorUsernames[4],
                Rating = 1,
                VideoId = FileHostingMock.VideoId,
                UserVote = null
            });

            videoReactions.Skip(2).First().Should().BeEquivalentTo(
                new VideoReactionWithUserVoteDto {
                    AuthorId = _authorIds[3],
                    Title = $"test-title-{_authorIds[3]}",
                    AuthorUsername = _authorUsernames[3],
                    Rating = 0,
                    VideoId = FileHostingMock.VideoId,
                    UserVote = -1
                }
            );
        }
    }
}
