using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.Discussion.Commands.PostDiscussionEntry;
using Livescore.Application.Livescore.Discussion.Queries.GetDiscussionsForFixture;
using Livescore.Application.Livescore.Discussion.Queries.GetMoreDiscussionEntries;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.Application.Common.Dto;

namespace Livescore.IntegrationTests.Livescore.Discussion.Queries {
    [Collection(nameof(LivescoreTestCollection))]
    public class Get_More_Discussion_Entries_Query_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly string _discussionId;

        public Get_More_Discussion_Entries_Query_Tests(Sut sut) {
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

            var result = _sut.SendRequest(
                new GetDiscussionsForFixtureQuery {
                    FixtureId = _fixtureId,
                    TeamId = _teamId
                }
            ).Result;

            _discussionId = result.Data.First(d => d.Name == "pre-match").Id;
        }

        [Fact]
        public async Task Should_Retrieve_Yet_Unretrieved_Discussion_Entries() {
            _sut.RunAs(userId: 2, username: "user-2");

            await Task.WhenAll(new[] {
                _sut.SendRequest(new PostDiscussionEntryCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    DiscussionId = _discussionId,
                    Body = "body-1"
                }),
                _sut.SendRequest(new PostDiscussionEntryCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    DiscussionId = _discussionId,
                    Body = "body-2"
                }),
                _sut.SendRequest(new PostDiscussionEntryCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    DiscussionId = _discussionId,
                    Body = "body-3"
                })
            });

            var result = await _sut.SendRequest(new GetMoreDiscussionEntriesQuery {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                DiscussionId = _discussionId,
                LastReceivedEntryId = null
            });

            var entries = result.Data;

            entries.Should().HaveCount(4);
        }
    }
}
