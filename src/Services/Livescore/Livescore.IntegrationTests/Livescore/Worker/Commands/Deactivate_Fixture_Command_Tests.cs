using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.Application.Livescore.Worker.Commands.UpdateFixturePrematch;
using Livescore.Application.Livescore.Worker.Commands.DeactivateFixture;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer;

namespace Livescore.IntegrationTests.Livescore.Worker.Commands {
    [Collection(nameof(LivescoreTestCollection))]
    public class Deactivate_Fixture_Command_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly string _participantKey1;
        private readonly string _participantKey2;

        public Deactivate_Fixture_Command_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            (_fixtureId, _teamId) = _sut.SeedWithDummyUpcomingFixture();

            _sut.SendRequest(
                new ActivateFixtureCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId
                }
            ).Wait();

            var fixture = _sut.GetSeededFixtureWithDummyPrematchData();
            var teamLineup = fixture.Lineups.First(l => l.TeamId == _teamId);
            var managerId = teamLineup.Manager.Id;
            _participantKey1 = $"m:{managerId}";
            var somePlayerId = teamLineup.StartingXI.First().Id;
            _participantKey2 = $"p:{somePlayerId}";

            _sut.SendRequest(
                new UpdateFixturePrematchCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    Fixture = fixture
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Deactivate_Fixture_And_Persist_Collected_Data() {
            _sut.RunAs(userId: 1, username: "user-1");

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey1,
                Rating = 8.15f
            });

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey2,
                Rating = 4.2f
            });

            _sut.RunAs(userId: 2, username: "user-2");

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey1,
                Rating = 6.71f
            });

            _sut.RunAsGuest();

            var result = await _sut.SendRequest(new DeactivateFixtureCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId
            });

            result.Should().BeOfType<VoidResult>().Which.Error.Should().BeNull();
        }
    }
}
