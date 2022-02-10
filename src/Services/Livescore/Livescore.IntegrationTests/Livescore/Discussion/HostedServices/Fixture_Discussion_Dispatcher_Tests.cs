using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.Discussion.Commands.PostDiscussionEntry;
using Livescore.Application.Livescore.Discussion.Queries.GetDiscussionsForFixture;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.IntegrationTests.Livescore.Discussion.Mocks;
using Livescore.Api.HostedServices;
using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Interfaces;

namespace Livescore.IntegrationTests.Livescore.Discussion.HostedServices {
    [Collection(nameof(LivescoreTestCollection))]
    public class Fixture_Discussion_Dispatcher_Tests : IDisposable {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly string _discussionId;

        public Fixture_Discussion_Dispatcher_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            _sut.StartHostedService<FixtureDiscussionDispatcher>();

            _sut.ExecWithService<IFixtureDiscussionBroadcaster, object>(
                fixtureDiscussionBroadcaster => {
                    ((FixtureDiscussionBroadcasterMock) fixtureDiscussionBroadcaster).Reset();
                    return Task.FromResult((object) null);
                })
                .Wait();

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

        public void Dispose() {
            _sut.StopHostedService<FixtureDiscussionDispatcher>();
        }

        [Fact]
        public async Task Should_Detect_Retrieve_And_Broadcast_New_Discussion_Entries() {
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

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            var updates = await _sut.ExecWithService<IFixtureDiscussionBroadcaster, IReadOnlyList<FixtureDiscussionUpdateDto>>(
                fixtureDiscussionBroadcaster => {
                    var fixtureDiscussionBroadcasterMock = (FixtureDiscussionBroadcasterMock) fixtureDiscussionBroadcaster;
                    return Task.FromResult(fixtureDiscussionBroadcasterMock.Updates);
                }
            );

            var discussionEntries = updates
                .Where(update => update.FixtureId == _fixtureId && update.TeamId == _teamId)
                .SelectMany(update => update.Entries);

            discussionEntries.Should().HaveCount(3);
        }
    }
}
