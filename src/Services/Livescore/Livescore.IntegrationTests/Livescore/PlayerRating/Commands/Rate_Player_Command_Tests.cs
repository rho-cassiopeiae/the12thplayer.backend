using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.Application.Livescore.Worker.Commands.UpdateFixturePrematch;

namespace Livescore.IntegrationTests.Livescore.PlayerRating.Commands {
    [Collection(nameof(LivescoreTestCollection))]
    public class Rate_Player_Command_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly string _participantKey;

        public Rate_Player_Command_Tests(Sut sut) {
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
            var managerId = fixture.Lineups.First(l => l.TeamId == _teamId).Manager.Id;
            _participantKey = $"m:{managerId}";

            _sut.SendRequest(
                new UpdateFixturePrematchCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    Fixture = fixture
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Update_Player_Total_Rating_And_Total_Voters_When_Rate_For_The_First_Time() {
            var result = await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey,
                Rating = 8.15f
            });

            result.Data.Should().BeEquivalentTo(
                new PlayerRatingDto {
                    ParticipantKey = _participantKey,
                    TotalRating = 8,
                    TotalVoters = 1
                }
            );
        }
    }
}
